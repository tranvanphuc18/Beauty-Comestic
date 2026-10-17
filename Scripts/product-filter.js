document.addEventListener('DOMContentLoaded', function () {
    'use strict';

    var form = document.getElementById('product-filter-form');
    if (!form) return;

    var chipBox = document.getElementById('product-filter-chips');
    var countBadge = document.getElementById('product-filter-count');

    function checkedInputs() {
        return Array.prototype.slice.call(form.querySelectorAll('input[type="checkbox"]:checked'));
    }

    function labelOf(input) {
        var label = form.querySelector('label[for="' + input.id + '"]');
        return label ? label.textContent.trim() : input.value;
    }

    // Hiện các "chip" của bộ lọc đang chọn; bấm vào chip để bỏ lọc đó
    function render() {
        var inputs = checkedInputs();
        chipBox.textContent = '';

        inputs.forEach(function (input) {
            var chip = document.createElement('button');
            chip.type = 'button';
            chip.className = 'product-filter__chip';
            chip.textContent = labelOf(input) + ' \u00D7';
            chip.setAttribute('aria-label', 'Bỏ chọn ' + labelOf(input));
            chip.addEventListener('click', function () {
                input.checked = false;
                form.submit();
            });
            chipBox.appendChild(chip);
        });

        chipBox.hidden = inputs.length === 0;

        if (countBadge) {
            countBadge.textContent = inputs.length;
            countBadge.classList.toggle('d-none', inputs.length === 0);
        }
    }

    // Không chặn submit nữa: form gửi GET về /Product/Index để server lọc
    render();
});
