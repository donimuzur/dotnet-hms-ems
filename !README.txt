Login: 
- Viewer: FULRICKA, FRATRIAN, FLESTARI, SNOVELIA, YLISTYON, DYULIANI
- User:SSUSANTI, RSUSANTI, ESUSANTI, CSUSANTO, KKHOTIMAH, DRAHAYU
- POA:DSANTOSO, NEFFENDI, AHARINI,SHAMIDAH,HARTATI1
- Admin: BKUMARA, CWIRADAN, HCHANDRA1
- POA Manager: NPERMAT1, FPAMARTA, FHADI

Login Route: /Login
Logout Route: /Login/Logout

SVN Path: http://192.168.10.38:123/svn/sampoerna-ems

[EMS Sampoerna]

Steps to add modules:
- Add Main Menu(s) to Home (Optional) => "~/Views/Home/Index.cshtml"
- Add Menu(s) to Master Menu Sidebar (Additional Master Only) => "~/Views/Shared/_Layout.cshtml"
- Add Controller Route to Table PAGE
- Register PageID from table PAGE to class Enums on project EMS.Core
- Create new folder on ~/Views, named with same name as Controller name
- Create new Model represent view model structure for each module
- Create new DB table for the module
- Update Edmx file on project EMS.CustomService
- Create UnitofWork instance on class Repositories\UnitOfWork
- Create new Service class on namespace Services, extends service class to Repositories\GenericService
- Create new Model class on project EMS.Website package Models
- Create new mapper on class EMS.Website\WebsiteMapper.cs inside a new descriptive region (e.g. #region Finance Ratio Mapper)

