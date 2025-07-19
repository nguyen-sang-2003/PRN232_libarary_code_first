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

// Khởi tạo chức năng mượn sách
export function initializeBorrowFunction() {
    $('#borrowBtn').click(function() {
        var bookId = $(this).data('bookid');

        // Lấy token hiện tại
        const token = getCurrentToken();

        if (!token) {
            alert('Bạn cần đăng nhập để mượn sách!');
            window.location.href = '/login?return_url=' + encodeURIComponent(window.location.pathname);
            return;
        }

        // Disable nút để tránh click nhiều lần
        $(this).prop('disabled', true).text('Đang xử lý...');

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
                // Re-enable nút
                $('#borrowBtn').prop('disabled', false).text('← Borrowing');

                if (xhr.status === 401) {
                    alert('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại!');
                    window.location.href = '/login?return_url=' + encodeURIComponent(window.location.pathname);
                } else if (xhr.status === 400) {
                    alert('Mượn sách thất bại: ' + (xhr.responseText || 'Không còn bản sao nào khả dụng'));
                } else {
                    alert('Mượn sách thất bại: ' + (xhr.responseText || 'Có lỗi xảy ra'));
                }
            }
        });
    });
}

// Function để quay lại trang sách
export function goBack() {
    window.location.href = '/';
}
