(function () {
    "use strict";

    document.addEventListener("DOMContentLoaded", function () {
        setupPasswordToggles();
        setupRegisterForm();
    });

    // Hiện hoặc ẩn mật khẩu
    function setupPasswordToggles() {
        document.addEventListener("click", function (event) {
            const button = event.target.closest(".password-toggle");

            if (!button) {
                return;
            }

            event.preventDefault();

            const targetId = button.getAttribute("data-password-target");
            const input = document.getElementById(targetId);

            if (!input) {
                console.error("Không tìm thấy ô mật khẩu:", targetId);
                return;
            }

            const shouldShow = input.type === "password";

            input.type = shouldShow ? "text" : "password";

            button.setAttribute(
                "aria-label",
                shouldShow ? "Ẩn mật khẩu" : "Hiện mật khẩu"
            );

            button.setAttribute("aria-pressed", String(shouldShow));

            const eye = button.querySelector(".icon-eye");
            const eyeOff = button.querySelector(".icon-eye-off");

            if (eye) {
                eye.hidden = shouldShow;
            }

            if (eyeOff) {
                eyeOff.hidden = !shouldShow;
            }
        });
    }

    // Xử lý biểu mẫu đăng ký
    function setupRegisterForm() {
        const form = document.getElementById("registerForm");
        const password = document.getElementById("registerPassword");
        const confirmPassword = document.getElementById("confirmPassword");
        const confirmError = document.getElementById("confirmPasswordError");
        const strengthBar = document.getElementById("passwordStrengthBar");
        const strengthText = document.getElementById("passwordStrengthText");

        if (!form || !password || !confirmPassword) {
            return;
        }

        // Kiểm tra hai mật khẩu có giống nhau không
        function validatePasswordMatch() {
            const mismatch =
                confirmPassword.value.length > 0 &&
                password.value !== confirmPassword.value;

            confirmPassword.setCustomValidity(
                mismatch ? "Mật khẩu xác nhận không khớp." : ""
            );

            if (confirmError) {
                confirmError.textContent = mismatch
                    ? "Mật khẩu xác nhận chưa khớp."
                    : "";
            }

            return !mismatch;
        }

        // Đánh giá độ mạnh của mật khẩu
        function updatePasswordStrength() {
            const value = password.value;
            let score = 0;

            if (value.length >= 8) {
                score++;
            }

            if (/[a-z]/.test(value) && /[A-Z]/.test(value)) {
                score++;
            }

            if (/[0-9]/.test(value)) {
                score++;
            }

            if (/[^A-Za-z0-9]/.test(value)) {
                score++;
            }

            const widths = ["0%", "25%", "50%", "75%", "100%"];

            const labels = [
                "Độ mạnh mật khẩu",
                "Mật khẩu yếu",
                "Mật khẩu trung bình",
                "Mật khẩu khá mạnh",
                "Mật khẩu mạnh"
            ];

            const colors = [
                "#e9dce2",
                "#dc3545",
                "#fd7e14",
                "#e0a800",
                "#198754"
            ];

            const index = value.length === 0 ? 0 : score;

            if (strengthBar) {
                strengthBar.style.width = widths[index];
                strengthBar.style.backgroundColor = colors[index];
            }

            if (strengthText) {
                strengthText.textContent = labels[index];
            }
        }

        password.addEventListener("input", function () {
            updatePasswordStrength();
            validatePasswordMatch();
        });

        confirmPassword.addEventListener("input", function () {
            validatePasswordMatch();
        });

        // Chỉ chặn gửi form khi dữ liệu chưa hợp lệ; hợp lệ thì để form POST lên server
        form.addEventListener("submit", function (event) {
            const passwordsMatch = validatePasswordMatch();

            if (!form.checkValidity() || !passwordsMatch) {
                event.preventDefault();
                form.reportValidity();
            }
        });
    }
})();