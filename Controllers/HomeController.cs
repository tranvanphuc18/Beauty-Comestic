using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using nv3.Models;
using nv3.ViewModels;

namespace nv3.Controllers
{
    public class HomeController : Controller
    {
        private BeautyCosmeticDBEntities db = new BeautyCosmeticDBEntities();

        // ----- Cấu hình mục "Gợi ý dành cho bạn" -----
        private const int FeaturedCount = 8;

        // Trọng số điểm xếp hạng (tổng = 1.0). Muốn ưu tiên tiêu chí nào thì chỉnh ở đây.
        private const double WeightSold = 0.60;     // BÁN CHẠY   : tổng số lượng đã bán
        private const double WeightViews = 0.25;    // ƯA CHUỘNG  : lượt xem trang chi tiết
        private const double WeightRating = 0.15;   // ƯA CHUỘNG  : điểm đánh giá (0-5)

        public ActionResult Index()
        {
            List<int> topIds;
            Dictionary<int, string> badges;
            PickTopProducts(FeaturedCount, out topIds, out badges);

            // Nạp đầy đủ sản phẩm theo danh sách id đã xếp hạng, rồi giữ đúng thứ tự xếp hạng
            var loaded = db.Products
                           .Include(p => p.ProductDetails)
                           .Where(p => topIds.Contains(p.ProductId))
                           .ToList();

            var featured = topIds
                .Select(id => loaded.FirstOrDefault(p => p.ProductId == id))
                .Where(p => p != null)
                .ToList();

            var model = new HomeViewModel
            {
                Categories = db.Categories.OrderBy(c => c.CategoryId).ToList(),
                FeaturedProducts = featured,
                PickBadges = badges
            };
            return View(model);
        }

        /// <summary>
        /// Chọn top sản phẩm theo 2 tiêu chí: BÁN CHẠY và ĐƯỢC ƯA CHUỘNG.
        ///   điểm = 0.60 * (đã bán / đã bán cao nhất)
        ///        + 0.25 * (lượt xem / lượt xem cao nhất)
        ///        + 0.15 * (rating / 5)
        /// Sản phẩm còn hàng luôn được xếp trước; chỉ khi không đủ mới bù bằng sản phẩm hết hàng.
        /// badges: id sản phẩm -> "hot" (nổi bật vì bán chạy) hoặc "love" (nổi bật vì được ưa chuộng).
        /// </summary>
        private void PickTopProducts(int take, out List<int> ids, out Dictionary<int, string> badges)
        {
            // Chỉ lấy các số liệu cần thiết (1 câu truy vấn nhẹ), tính điểm ở bộ nhớ
            var stats = db.Products.Select(p => new
            {
                p.ProductId,
                p.Rating,
                p.ViewQuantity,
                Sold = p.ProductDetails.Sum(d => (int?)d.SoldQuantity) ?? 0,
                InStock = p.ProductDetails.Any(d => d.Quantity > 0)
            }).ToList();

            // Mốc chuẩn hóa chỉ tính trên sản phẩm CÒN HÀNG: một sản phẩm đã hết hàng với số liệu cực cao
            // sẽ không làm "loãng" điểm của các sản phẩm khách có thể mua ngay.
            var pool = stats.Where(s => s.InStock).ToList();
            if (pool.Count == 0) pool = stats;

            double maxSold = pool.Count > 0 ? pool.Max(s => s.Sold) : 0;
            double maxViews = pool.Count > 0 ? pool.Max(s => s.ViewQuantity) : 0;

            var ranked = stats.Select(s =>
            {
                double rating = Math.Max(0.0, Math.Min(5.0, (double)s.Rating));

                // Chia điểm thành 2 phần để biết sản phẩm nổi bật vì lý do gì
                double soldPart = maxSold > 0 ? WeightSold * Math.Min(1.0, s.Sold / maxSold) : 0;
                double lovedPart = (maxViews > 0 ? WeightViews * Math.Min(1.0, s.ViewQuantity / maxViews) : 0)
                                   + WeightRating * rating / 5.0;

                return new
                {
                    s.ProductId,
                    s.Sold,
                    s.ViewQuantity,
                    s.InStock,
                    SoldPart = soldPart,
                    LovedPart = lovedPart,
                    Score = soldPart + lovedPart
                };
            })
            .OrderByDescending(x => x.InStock)          // còn hàng trước
            .ThenByDescending(x => x.Score)             // điểm cao trước
            .ThenByDescending(x => x.Sold)              // bằng điểm: bán nhiều hơn trước
            .ThenByDescending(x => x.ViewQuantity)      // rồi đến xem nhiều hơn
            .ThenBy(x => x.ProductId)                   // cuối cùng: kết quả ổn định, không nhảy lung tung
            .Take(take)
            .ToList();

            ids = ranked.Select(x => x.ProductId).ToList();

            // Chỉ gắn nhãn khi sản phẩm thật sự có số liệu (đã bán hoặc có người xem),
            // tránh gắn "được ưa chuộng" cho sản phẩm mới chỉ có rating mặc định.
            badges = ranked
                .Where(x => x.InStock && (x.Sold > 0 || x.ViewQuantity > 0))
                .ToDictionary(x => x.ProductId,
                              x => (x.Sold > 0 && x.SoldPart >= x.LovedPart) ? "hot" : "love");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
