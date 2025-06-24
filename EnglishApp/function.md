1. Authentication (Đăng ký, Đăng nhập, OTP, Reset password, Đổi mật khẩu)

API Endpoint	Method	Chức năng	Ghi chú
/api/auth/register	POST	Đăng ký tài khoản	Body: Email, Password, UserName
/api/auth/send-otp	POST	Gửi mã OTP xác thực qua email	Dùng cho register/quên mật khẩu
/api/auth/verify-otp	POST	Xác thực OTP khi đăng ký hoặc reset mật khẩu	
/api/auth/login	POST	Đăng nhập (email/username, password)	Trả về JWT token
/api/auth/forgot-password	POST	Yêu cầu quên mật khẩu, gửi OTP reset	
/api/auth/reset-password	POST	Đổi mật khẩu mới qua OTP	
/api/auth/change-password	POST	Đổi mật khẩu khi đã đăng nhập	Yêu cầu login
/api/auth/logout	POST	Đăng xuất (nếu dùng refresh token, blacklist token v.v.)	Optional, chỉ cần nếu có refresh token

2. User (Thông tin cá nhân, Avatar, Role, Quản lý user cho Admin)
API Endpoint	Method	Chức năng	Ghi chú
/api/user/profile	GET	Lấy thông tin cá nhân	
/api/user/profile	PUT	Cập nhật thông tin cá nhân	Body: FullName, Birthday...
/api/user/avatar	POST	Upload ảnh đại diện	Multipart/form-data
/api/user/avatar	DELETE	Xóa ảnh đại diện	
/api/user/roles	GET	Xem quyền hiện tại của user	
/api/user/my-progress	GET	Lấy tiến độ học của user (tổng hợp)	
/api/user/my-results	GET	Lấy lịch sử làm bài tập	

Chức năng quản lý user (Admin):
| /api/admin/users | GET | Danh sách tất cả user | Có filter, phân trang |
| /api/admin/users/{id} | GET | Lấy chi tiết user | |
| /api/admin/users | POST | Tạo mới user (admin thêm user) | |
| /api/admin/users/{id} | PUT | Cập nhật thông tin user | |
| /api/admin/users/{id} | DELETE | Xóa user | |
| /api/admin/users/{id}/roles | PUT | Cập nhật role cho user | Body: List roleId |
| /api/admin/users/{id}/reset-password| POST | Reset mật khẩu user về mặc định | |

3. Category (Quản lý chủ đề, danh mục học)
API Endpoint	Method	Chức năng	Ghi chú
/api/categories	GET	Lấy danh sách chủ đề	
/api/categories/{id}	GET	Lấy chi tiết chủ đề	

Admin:
| /api/categories | POST | Thêm chủ đề | |
| /api/categories/{id} | PUT | Sửa chủ đề | |
| /api/categories/{id} | DELETE | Xóa chủ đề | |

4. Lesson (Bài học, thuộc category)
API Endpoint	Method	Chức năng	Ghi chú
/api/lessons	GET	Lấy danh sách bài học	Query: categoryId, level
/api/lessons/{id}	GET	Lấy chi tiết bài học	
/api/lessons/{id}/contents	GET	Lấy nội dung bài học	
/api/lessons/{id}/exercises	GET	Lấy danh sách bài tập của bài học	
/api/lessons/popular	GET	Danh sách bài học phổ biến, gợi ý	Optional

Admin:
| /api/lessons | POST | Thêm bài học | |
| /api/lessons/{id} | PUT | Sửa bài học | |
| /api/lessons/{id} | DELETE | Xóa bài học | |

5. Lesson Content (Nội dung bài học, nhiều loại: text/audio/video/quiz)
API Endpoint	Method	Chức năng	Ghi chú
/api/lesson-contents/{id}	GET	Lấy chi tiết nội dung bài học	
/api/lessons/{lessonId}/contents	GET	Lấy danh sách nội dung bài học	

Admin:
| /api/lessons/{lessonId}/contents | POST | Thêm nội dung vào bài học | |
| /api/lesson-contents/{id} | PUT | Sửa nội dung | |
| /api/lesson-contents/{id} | DELETE | Xóa nội dung | |

6. Exercise (Bài tập, quiz - thuộc lesson)
API Endpoint	Method	Chức năng	Ghi chú
/api/exercises/{id}	GET	Lấy chi tiết bài tập	
/api/lessons/{lessonId}/exercises	GET	Lấy danh sách bài tập theo bài học	

Admin:
| /api/lessons/{lessonId}/exercises | POST | Thêm bài tập vào bài học | |
| /api/exercises/{id} | PUT | Sửa bài tập | |
| /api/exercises/{id} | DELETE | Xóa bài tập | |

7. Exercise Option (Đáp án cho quiz)
API Endpoint	Method	Chức năng	Ghi chú
/api/exercises/{exerciseId}/options	GET	Lấy đáp án của bài tập	

Admin:
| /api/exercises/{exerciseId}/options | POST | Thêm đáp án cho bài tập | |
| /api/exercise-options/{optionId} | PUT | Sửa đáp án | |
| /api/exercise-options/{optionId} | DELETE | Xóa đáp án | |

8. Làm bài, lưu kết quả (UserExerciseResult, Progress)
API Endpoint	Method	Chức năng	Ghi chú
/api/exercises/{id}/submit	POST	Nộp đáp án, chấm điểm, trả về kết quả	Body: selectedOptionId
/api/user/lesson-progress/{lessonId}	GET	Lấy tiến độ bài học cụ thể	
/api/user/lesson-progress	GET	Lấy toàn bộ tiến độ học	
/api/user/lesson-progress/{lessonId}	PUT	Cập nhật tiến độ học (status, score, ...)	
/api/user/exercise-results	GET	Lấy lịch sử làm bài tập	

9. Thống kê & Báo cáo (Statistics, Analytics)
API Endpoint	Method	Chức năng	Ghi chú
/api/user/statistics	GET	Thống kê kết quả học cá nhân	
/api/admin/statistics	GET	Thống kê tổng thể cho admin	

10. Role & Permission (Phân quyền, quyền động)
API Endpoint	Method	Chức năng	Ghi chú
/api/user/roles	GET	Xem role của user hiện tại	
/api/admin/roles	GET	Lấy danh sách role	
/api/admin/roles	POST	Tạo mới role	
/api/admin/roles/{id}	PUT	Sửa thông tin role	
/api/admin/roles/{id}	DELETE	Xóa role	

11. Quản lý claim (quyền mở rộng, optional nếu dùng Identity)
API Endpoint	Method	Chức năng	Ghi chú
/api/admin/claims	GET	Danh sách claim	
/api/admin/claims	POST	Thêm claim	
/api/admin/claims/{id}	PUT	Sửa claim	
/api/admin/claims/{id}	DELETE	Xóa claim	
/api/admin/userclaims	GET	Lấy claim của user	
/api/admin/roleclaims	GET	Lấy claim của role	

12. OTP & Bảo mật tài khoản (phục vụ xác thực 2 lớp, đổi mật khẩu, quên mật khẩu...)
API Endpoint	Method	Chức năng	Ghi chú
/api/auth/send-otp	POST	Gửi mã OTP qua email	Đăng ký, quên pass
/api/auth/verify-otp	POST	Xác thực OTP	
/api/auth/2fa-enable	POST	Kích hoạt xác thực 2 lớp	Optional, nâng cao
/api/auth/2fa-disable	POST	Tắt xác thực 2 lớp	Optional, nâng cao