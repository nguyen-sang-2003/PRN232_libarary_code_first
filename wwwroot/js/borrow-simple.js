// Function để lấy token từ localStorage hoặc cookie
function getCurrentToken() {
    let token = localStorage.getItem("token");
    if (!token) {
        // Lấy từ cookie nếu không có trong localStorage
        const cookies = document.cookie.split(';');
        for (let cookie of cookies) {
            const [name, value] = cookie.trim().split('=');
            if (name === 'token') {
                token = value;
                break;
            }
        }
    }
    return token;
}

// Khởi tạo sự kiện cho các nút mượn sách
$(document).ready(function() {
    $('.borrowBtn').click(function() {
        var bookId = $(this).data('bookid');

        // Lấy token hiện tại
        const token = getCurrentToken();

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
});
