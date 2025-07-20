import { get_current_token } from '../utils.mjs';

export function initializeBorrowButtons() {
    $('.borrowBtn').click(function() {
        var bookId = $(this).data('bookid');

        // Lấy token hiện tại
        const token = get_current_token();

        if (!token) {
            alert('Bạn cần đăng nhập để mượn sách!');
            window.location.href = '/login';
            return;
        }

        $.ajax({
            url: '/api/borrowing/rentals/request?bookId=' + bookId,
            type: 'POST',
            headers: {
                'Authorization': 'Bearer ' + token,
                'Content-Type': 'application/json'
            },
            success: function(response) {
                alert('Mượn sách thành công!');
                window.location.href = '/'; // Chuyển về trang chủ
            },
            error: function(xhr) {
                if (xhr.status === 401) {
                    alert('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại!');
                    window.location.href = '/login';
                } else {
                    alert('Mượn sách thất bại: ' + (xhr.responseText || 'Có lỗi xảy ra'));
                }
            }
        });
    });
}
