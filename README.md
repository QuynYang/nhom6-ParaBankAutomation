# 📌 ParaBank Automation Testing – Nhóm 6

---

## 📖 1. Giới thiệu dự án

Dự án **ParaBank Automation Testing** là hệ thống kiểm thử tự động cho website ngân hàng demo ParaBank, được xây dựng nhằm mục đích:

* Tự động hóa các test case quan trọng
* Giảm thời gian kiểm thử thủ công
* Tăng độ chính xác khi kiểm thử
* Áp dụng các kỹ thuật kiểm thử hiện đại (Automation Testing)

---

## 🎯 2. Mục tiêu dự án

* Xây dựng framework automation theo **Page Object Model (POM)**
* Tự động hóa các chức năng chính của hệ thống
* Sử dụng **Data-driven testing** với Excel
* Tạo báo cáo kết quả test
* Tối ưu khả năng maintain và mở rộng test

---

## 🧰 3. Công nghệ sử dụng

| Công nghệ          | Mô tả                   |
| ------------------ | ----------------------- |
| Selenium WebDriver | Tự động hóa trình duyệt |
| C# (.NET)          | Ngôn ngữ lập trình      |
| NUnit              | Framework test          |
| Visual Studio      | IDE                     |
| GitHub             | Quản lý source          |
| Excel              | Quản lý test data       |

---

## 🏗️ 4. Kiến trúc framework

Dự án được xây dựng theo mô hình:

### 🔹 Page Object Model (POM)

* Mỗi trang web = 1 class
* Tách biệt:

  * UI (Page)
  * Logic (Test)
  * Data (Excel)

---

### 📂 Cấu trúc project

```
ParaBankAutomation/
│── Helpers/          # Chứa logic đọc ghi excel
│── Pages/              # Chứa Page Object
│── Reports/            # Báo cáo test
│── Test/            # Chứa các test script theo các chức năng
│── README.md
```

---

## 🧪 5. TEST PLAN

---

### 5.1 Objective (Mục tiêu kiểm thử)

* Xác minh hệ thống hoạt động đúng yêu cầu
* Phát hiện lỗi trong các chức năng chính
* Đảm bảo UI và flow hoạt động ổn định

---

### 5.2 Scope (Phạm vi kiểm thử)

### ✅ In Scope

* Register
* Login / Logout

---

### 5.3 Test Strategy

Áp dụng các loại test:

* Functional Testing
* UI Testing
* Smoke Testing

Kỹ thuật sử dụng:

* Page Object Model (POM)
* Phân vùng tương đương
* Logic điều kiện
* Kiểm tra giá trị biên
* Trạng thái hệ thống (bảo mật, tính toàn vẹn,...)
* Data-driven testing (Excel)

---

### 5.4 Test Environment

| Thành phần | Giá trị       |
| ---------- | ------------- |
| OS         | Windows 10/11 |
| Browser    | Chrome        |
| Tool       | Selenium      |
| IDE        | Visual Studio |

---

## 📊 6. Test Scenarios

---

### 🔐 Login

* Kiểm tra GUI login
* Kiểm tra Phân vùng tương đương (valid/invalid) và logic điều kiện
* Kiểm tra giá trị biên (password độ dài tối thiểu)
* Kiểm tra trạng thái hệ thống

---

### 📝 Register

* Kiểm tra GUI register
* Kiểm tra Phân vùng tương đương (valid/invalid) và logic điều kiện
* Kiểm tra giá trị biên (password độ dài tối thiểu)
* Kiểm tra trạng thái hệ thống

---


## ▶️ 7. Hướng dẫn chạy project

### Bước 1: Clone repo

```bash
git clone https://github.com/QuynYang/nhom6-ParaBankAutomation
```

---

### Bước 2: Mở bằng Visual Studio

---

### Bước 3: Cài thư viện

* Selenium.WebDriver
* NUnit
* NUnit3TestAdapter

---

### Bước 4: Run test

* Mở Test Explorer
* Chạy toàn bộ test

---

## 👥 8. Thành viên nhóm

| Tên      | Độ đóng góp dự án    |
| -------- | ---------- |
| Vũ Ngọc Quỳnh Giang | 100%     |


---

## 🚀 9. Hướng phát triển

* Tích hợp CI/CD (GitHub Actions)
* Tạo dashboard report
* Mở rộng coverage
* Kết hợp AI testing

---
