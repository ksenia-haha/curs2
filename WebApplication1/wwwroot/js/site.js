// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('click', function (event) {
    let button = event.target.closest('.delete-btn');

    if (button && button.tagName === 'A') {
        event.preventDefault();

        // Можно использовать разное сообщение в зависимости от атрибута
        let message = button.getAttribute('data-confirm-message') || 'Вы действительно хотите удалить этот элемент?';

        let userConfirmed = confirm(message);

        if (userConfirmed) {
            window.location.href = button.href;
        }
    }
});

