(function () {
    "use strict";

    const CART_KEY = "beautyCart";

    function readCart() {
        try {
            const data = JSON.parse(localStorage.getItem(CART_KEY) || "[]");
            return Array.isArray(data) ? data : [];
        } catch (error) {
            return [];
        }
    }

    function updateCartBadge() {
        const badge = document.getElementById("headerCartCount");

        if (!badge) {
            return;
        }

        const cart = readCart();
        const total = cart.reduce(function (sum, item) {
            const quantity = Number(item.quantity) || 0;
            return sum + Math.max(0, quantity);
        }, 0);

        badge.textContent = total > 99 ? "99+" : String(total);
        badge.setAttribute("aria-label", total + " sản phẩm trong giỏ hàng");
    }

    function setupMobileMenu() {
        const button = document.getElementById("menuToggle");
        const nav = document.getElementById("mainNav");

        if (!button || !nav) {
            return;
        }

        button.addEventListener("click", function () {
            const isOpen = nav.classList.toggle("is-open");

            button.setAttribute("aria-expanded", String(isOpen));
            button.setAttribute(
                "aria-label",
                isOpen ? "Đóng menu" : "Mở menu"
            );
        });

        nav.addEventListener("click", function (event) {
            if (event.target.closest("a")) {
                nav.classList.remove("is-open");
                button.setAttribute("aria-expanded", "false");
                button.setAttribute("aria-label", "Mở menu");
            }
        });

        window.addEventListener("resize", function () {
            if (window.innerWidth > 767) {
                nav.classList.remove("is-open");
                button.setAttribute("aria-expanded", "false");
                button.setAttribute("aria-label", "Mở menu");
            }
        });
    }

    document.addEventListener("DOMContentLoaded", function () {
        setupMobileMenu();
        updateCartBadge();
    });

    window.addEventListener("storage", function (event) {
        if (event.key === CART_KEY) {
            updateCartBadge();
        }
    });

    window.addEventListener("beautyCartUpdated", updateCartBadge);
})();5