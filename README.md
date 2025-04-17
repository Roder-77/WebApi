# 資深平台開發測驗

> DB Table 是由 EFCore Migration 自動產生，SQL檔案提供參考

### 主要架構

1. Common 類別庫負責共用性比較強的 Enum、Helper
2. Models 類別庫負責各式 Model、Attribute、Exception
3. Service 類別庫負責各式服務相關 Service、Repository
4. 專案 WebApi & MVC
   * Controller(WebApi) or PageModel(MVC)
     * 負責接收 HTTP 請求，並將請求轉發到 Service 層
   * Service
     * 負責處理商業邏輯，並將結果返回給 Controller
   * Repository
     * 負責與資料庫進行交互，並將資料庫的結果返回給 Service 層

## 產品 API
#### WebApi 專案

📄 News

| Column Name | Type          | Primary Key | Nullable | Foreign Key | Remark    |
|-------------|---------------|-------------|----------|-------------|-----------|
| Id          | int           | ✅          | ❌       |             | IDENTITY  |
| Title       | nvarchar(100) |             | ❌       |             |           |
| Summary     | nvarchar(max) |             | ❌       |             |           |
<br>

📄 Product

| Column Name | Type          | Primary Key | Nullable | Foreign Key | Remark    |
|-------------|---------------|-------------|----------|-------------|-----------|
| Id          | int           | ✅          | ❌       |             | IDENTITY  |
| Name        | nvarchar(50)  |             | ❌       |             |           |
| Intro       | nvarchar(500) |             | ❌       |             |           |
<br>

📄 ProductImage

| Column Name | Type          | Primary Key | Nullable | Foreign Key | Remark         |
|-------------|---------------|-------------|----------|-------------|----------------|
| Id          | int           | ✅          | ❌       |             | IDENTITY       |
| ProductId   | int           |             | ❌       | ✅          | 關聯至 Product |
| Url         | nvarchar(300) |             | ❌       |             |                |
<br>

📄 ProductNewsRelation

| Column Name | Type        | Primary Key | Nullable | Foreign Key | Remark          |
|-------------|-------------|-------------|----------|-------------|-----------------|
| Id          | int         | ✅          | ❌       |             | IDENTITY        |
| ProductId   | int         |             | ❌       | ✅          | 關聯至 Product  |
| NewsId      | int         |             | ❌       | ✅          | 關聯至 News     |
<br>

📄 ProductSpec

| Column Name | Type          | Primary Key | Nullable | Foreign Key | Remark         |
|-------------|---------------|-------------|----------|-------------|----------------|
| Id          | int           | ✅          | ❌       |             | IDENTITY       |
| ProductId   | int           |             | ❌       | ✅          | 關聯至 Product |
| Name        | nvarchar(50)  |             | ❌       |             |                |
| Description | nvarchar(500) |             | ❌       |             |                | 


### 說明
1. 主要是由 Product 和 ProductImage、ProductSpec 進行一對多關聯，ProductNewsRelation 則是與 Product、News 進行多對多關聯。
2. ProductImage、ProductSpec，實際上沒有限制上傳筆數，會在 Service 層從資料庫取資料的同時篩出指定要的筆數，進階情況可再追加依照排序等其他條件去變更從資料庫撈資料的處理方式，達到可以於後台調整要顯示的內容。
3. ProductNewsRelation 則是負責建立 Product、News 之間的橋樑，實現產品頁可以關聯 News，News 頁也可以關聯產品，目前沒有指定筆數，撈取時沒特別處理，會直接撈取所有相關聯的 News。
4. `呼叫外部 API 取得與此產品相關的 AI 推薦標籤`，則是有另開一個取得商品標籤 API 模擬結果
5. 目前已經有預設的資料 (ProductId：1 & 2)，可以透過 Swagger 介面呼叫取得商品 API 測試
<br>

## 使用者角色權限
#### WebApi 專案

📄 Member

| Column Name | Type         | Primary Key | Nullable | Foreign Key | Remark    |
|-------------|--------------|-------------|----------|-------------|-----------|
| Id          | int          | ✅          | ❌       |             | IDENTITY  |
| Name        | nvarchar(20) |             | ❌       |             |           |
| Mobile      | nvarchar(20) |             | ❌       |             |           |
| Email       | nvarchar(50) |             | ❌       |             |           |
| CreateTime  | bigint       |             | ❌       |             |           |
| Creator     | int          |             | ❌       |             |           |
| UpdateTime  | bigint       |             | ❌       |             |           |
| Updater     | int          |             | ❌       |             |           |
<br>

📄 Menu

| Column Name | Type          | Primary Key | Nullable | Foreign Key | Remark    |
|-------------|---------------|-------------|----------|-------------|-----------|
| Id          | int           | ✅          | ❌       |             | IDENTITY  |
| Name        | nvarchar(50)  |             | ❌       |             |           |
<br>

📄 Permission

| Column Name | Type          | Primary Key | Nullable | Foreign Key | Remark    |
|-------------|---------------|-------------|----------|-------------|-----------|
| Id          | int           | ✅          | ❌       |             | IDENTITY  |
| Name        | nvarchar(50)  |             | ❌       |             |           |
<br>

📄 MemberPermission

| Column Name  | Type | Primary Key | Nullable | Foreign Key | Remark              |
|--------------|------|-------------|----------|-------------|---------------------|
| Id           | int  | ✅          | ❌       |             | IDENTITY            |
| MemberId     | int  |             | ❌       | ✅          | 關聯至 Member       |
| PermissionId | int  |             | ❌       | ✅          | 關聯至 Permission   |
<br>

📄 PermissionMenuRelation

| Column Name  | Type | Primary Key | Nullable | Foreign Key | Remark              |
|--------------|------|-------------|----------|-------------|---------------------|
| Id           | int  | ✅          | ❌       |             | IDENTITY            |
| MenuId       | int  |             | ❌       | ✅          | 關聯至 Menu         |
| PermissionId | int  |             | ❌       | ✅          | 關聯至 Permission   |
| AllowCreate  | bit  |             | ❌       |             |                     |
| AllowRead    | bit  |             | ❌       |             |                     |
| AllowUpdate  | bit  |             | ❌       |             |                     |
| AllowDelete  | bit  |             | ❌       |             |                     |

### 說明
1. 主要是由 MemberPermission 與 Member、Permission 進行多對多關聯，以及 PermissionMenuRelation 與 Permission、Menu 進行多對多關聯，來達到 Memeber 可以設定多個 Permission 且每個 Permission 可以管理多個 Menu，再依照 PermissionMenuRelation 對應設定每個 Permission & Menu 對應的 CRUD 狀態
2. 目前已經有預設的資料 (MemberId：1 & MenuId：1 ~ 5)，可以透過 Swagger 介面呼叫取得會員選單 API 測試 (加分題)
<br>

## SEO
#### MVC 專案

* [Local Mirror File] is true:
  * og-iamge path is /File/image/{Name}-ogimge{Ext}
  * [Ext] is empty: og-iamge path is /File/image/{Name}{Ext} `這段有點不確定，Ext 已經是 empty，但還是透過 Ext 組成，目前還是先照此規格處理`
* [Local Mirror File] is false:
  * og-iamge path is /File/image/{Name}-ogimge{Local Ext}
  * [Local Ext] is empty: og-iamge path is /File/image/{Name}{Ext} `這段有點不確定，Ext 也有可能是 empty，目前還是先照此規格處理`

> 因為不太確定會以什麼欄位為基準去撈資料，所以目前的設定是透過 Id 來撈取

### 運作方式
1. MVC/PageModels/BasePageModel > 負責控制進入時撈取 SEO 資料，具體撈取方式可以依照需求調整，也可以依照不同類型分別繼承 PageModel 處理，並透過 ViewData 的方式供 Layout、Partial View、Page 頁面使用，目前沒有指定方式先固定撈取 id：1 (依照範例可以調整範圍為 id：1 ~ 5)
2. MVC/Pages/index.cshtml > 負責處理首頁 Body，會渲染在 Layout RenderBody 的位置
3. MVC/Pages/Shared/_MetaTag.cshtml > 負責處理 Meta Tag，如果日後有區分不同的 Layout，但邏輯相同可直接套用
4. Common/Models/ViewModels/SEOVM > 有新增 Property ImagePath 處理 Local Mirror File 的情境，對於頁面使用上的 Nullable 處理也比較方便