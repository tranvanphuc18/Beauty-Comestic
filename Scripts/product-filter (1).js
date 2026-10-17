
document.addEventListener('DOMContentLoaded', function () {
    'use strict';

    var form = document.getElementById('product-filter-form');
    if (!form) return;

    var panel = document.getElementById('product-filter');
    var chipBox = document.getElementById('product-filter-chips');
    var countBadge = document.getElementById('product-filter-count');

    function checkedInputs() {
        return Array.prototype.slice.call(form.querySelectorAll('input[type="checkbox"]:checked'));
    }

    function labelOf(input) {
        var label = form.querySelector('label[for="' + input.id + '"]');
        return label ? label.textContent.trim() : input.value;
    }

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
                render();
            });
            chipBox.appendChild(chip);
        });

        chipBox.hidden = inputs.length === 0;

        if (countBadge) {
            countBadge.textContent = inputs.length;
            countBadge.classList.toggle('d-none', inputs.length === 0);
        }
    }

    function closePanelOnMobile() {
        if (!panel || !window.bootstrap) return;
        var instance = window.bootstrap.Offcanvas.getInstance(panel);
        if (instance) instance.hide();
    }

    form.addEventListener('submit', function (e) {
        e.preventDefault();
        render();
        closePanelOnMobile();
    });

    form.addEventListener('reset', function () {
        setTimeout(render, 0);
    });

    render();
});
