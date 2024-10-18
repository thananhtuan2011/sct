namespace API_SoCongThuong.Classes
{
    public class Constants
    {
    }
    public static class AppSessionConstants
    {
        public static int Web = 1;
        public static int Mobile = 2;
    }
    public static class ActionType_Const
    {
        public static string LOGIN = "LOGIN";
        public static string LOGOUT = "LOGOUT";
        public static string VIEW = "VIEW";
        public static string CREATE = "CREATE";
        public static string UPDATE = "UPDATE";
        public static string DELETE = "DELETE";
    }
    public static class ErrCode_Const
    {
        public const int SUCCESS = 0;
        public const int KEY_FILTER_NOT_EXISTS = 1;
        public const int EXCEPTION_API = 2;
        public const int PROPERTY_IS_NULL_OR_EMPTY = 3;

        /// <summary>
        /// Cannot find data form id 
        /// </summary>
        public const int CANNOT_FIND_DATA_BY_QUERY = 4;
        public const int PROPERTY_IS_REQUIRED = 5;
        public const int PROPERTY_IS_INVALID = 6;
        public const int KEY_PROPS_NOT_FOUND = 7;
        public const int TYPE_OF_FILE_UPLOAD_INVALID = 8;
        public const int SIZE_FILE_TOO_BIG = 9;
        public const int NO_FILE_TO_UPLOAD = 10;
        public const int FILE_FORMAT_INVALID = 11;
        public const int LOGIN_SESSION_EXPIRED = 12;
        public const int FILE_READ_EXCEPTION = 14;
        public const int USER_NOT_FOUND = 15;
        public const int JWT_CANNOT_REFRESH_TOKEN = 16;
        public const int JWT_EXCEPTION_ERROR = 17;
        public const int JWT_INVALID_TOKEN = 18;
        public const int JWT_TOKEN_IS_REQUIRED = 19;
        public const int SQL_QUERY_HAS_ERROR = 20;
        public const int SQL_QUERY_NO_ROW = 21;
        public const int REQUEST_PARAMS_NULL = 22;
        public const int SQL_QUERY_ERROR_RETURN_TBL = 23;
        public const int SQL_QUERY_CANNOT_PAGINATE = 24;
        public const int SQL_INSERT_FAILED = 25;
        public const int PROPERTY_IS_ENOUGH_COUT = 26;

    }

    public static class ErrMsg_Const
    {
        private static readonly Dictionary<int, string> Dic_Error =
             new Dictionary<int, string>
             {
                { ErrCode_Const.SUCCESS, String.Empty}//query search not has in api
                ,{ ErrCode_Const.KEY_FILTER_NOT_EXISTS, "Key filter is not exists" }//query search not has in api
                ,{ ErrCode_Const.EXCEPTION_API, "Error Exception! cannot do this action" }//query search not has in api
                ,{ ErrCode_Const.PROPERTY_IS_NULL_OR_EMPTY, "Property cannot be null or empty" }//query search not has in api
                ,{ ErrCode_Const.CANNOT_FIND_DATA_BY_QUERY, "Data is empty" }//query search not has in api
                ,{ ErrCode_Const.PROPERTY_IS_REQUIRED, "Property is required" }//query search not has in api
                ,{ ErrCode_Const.PROPERTY_IS_INVALID, "Property is invalid" }//query search not has in api
                ,{ ErrCode_Const.KEY_PROPS_NOT_FOUND, "Data not found" }//query search not has in api
                ,{ ErrCode_Const.TYPE_OF_FILE_UPLOAD_INVALID, "Type of file upload invalid" }//query search not has in api
                ,{ ErrCode_Const.SIZE_FILE_TOO_BIG, "Size of file is too big" }//query search not has in api
                ,{ ErrCode_Const.NO_FILE_TO_UPLOAD, "No file to upload" }//query search not has in api
                ,{ ErrCode_Const.FILE_FORMAT_INVALID, "Format file is invalid" }//query search not has in api
                ,{ ErrCode_Const.LOGIN_SESSION_EXPIRED, "Login session is expired" }//query search not has in api
                ,{ ErrCode_Const.FILE_READ_EXCEPTION, "Read file on server has exceptions" }//query search not has in api
                ,{ ErrCode_Const.USER_NOT_FOUND, "User not found" }//query search not has in api
                ,{ ErrCode_Const.JWT_CANNOT_REFRESH_TOKEN, "Cannot get refresh token" }//query search not has in api
                ,{ ErrCode_Const.JWT_EXCEPTION_ERROR, "Error Exception when get token" }//query search not has in api
                ,{ ErrCode_Const.JWT_INVALID_TOKEN, "Token base is invalid" }//query search not has in api
                ,{ ErrCode_Const.JWT_TOKEN_IS_REQUIRED, "Token is required" }//query search not has in api
                ,{ ErrCode_Const.SQL_QUERY_HAS_ERROR, "Cannot get data" }//query search not has in api
                ,{ ErrCode_Const.SQL_QUERY_NO_ROW, "Data is empty" }//query search not has in api
                ,{ ErrCode_Const.REQUEST_PARAMS_NULL, "Request param is empty" }//query search not has in api
                ,{ ErrCode_Const.SQL_QUERY_ERROR_RETURN_TBL, "Data is not enough to query" }//query search not has in api
                ,{ ErrCode_Const.SQL_QUERY_CANNOT_PAGINATE, "Data cannot paginate" }//query search not has in api
                ,{ ErrCode_Const.SQL_INSERT_FAILED, "Cannot save data" }//query search not has in api
                ,{ ErrCode_Const.PROPERTY_IS_ENOUGH_COUT, "Code to remove cannot enough" }//query search not has in api
             };

        /// <summary>
        /// Get error message from error 
        /// </summary>
        /// <param name="pErrorCode">From ErrCode_Constant </param>
        /// <returns></returns>
        //public string GetMsg(int pErrorCode)
        //{
        //    try
        //    {
        //        return Dic_Error.ContainsKey(pErrorCode) ? Dic_Error[pErrorCode] : "Error has been not defined";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "Error_Undefined";
        //    }
        //}

        public static string GetMsg(int pErrorCode)
        {
            if (Dic_Error.TryGetValue(pErrorCode, out string errMsg))
            {
                return errMsg;
            }
            return "Error has not been defined";
        }
    }

    public static class Action_Status
    {
        public static string SUCCESS = "Thành công";
        public static string FAIL = "Thất bại";
    }
    public static class NameCategory_Const
    {

        //Danh mục dùng chung
        public static string DISTRICT = "Quản lý danh mục huyện";
        public static string COMMUNE = "Quản lý danh mục xã";
        public static string INDUSTRY = "Quản lý danh mục ngành nghề";
        public static string COUNTRY = "Quản lý danh mục quốc gia xuất - nhập khẩu";
        public static string TYPE_OF_MARKET = "Quản lý danh mục loại hình chợ";
        public static string TYPE_OF_BUSINESS = "Quản lý danh mục doanh nghiệp";

        public static string ENERGY_INDUSTRY = "Quản lý danh mục lĩnh vực hoạt động điện lực";
        public static string TYPE_OF_ENERGY = "Quản lý danh mục loại hình năng lương";
        public static string TYPE_OF_TRADE_PROMOTION = "Quản lý danh mục loại hình xúc tiến thương mại";
        public static string TRADE_PROMOTION_PROJECT = "Quản lý danh mục đề án xúc tiến thương mại";
        public static string ADMIN_FOMALITIES = "Quản lý danh mục thủ tục hành chính";
        public static string TYPE_OF_PROFESSION = "Quản lý danh mục loại ngành nghề";

        public static string BUSINESS = "Quản lý danh mục doanh nghiệp";
        public static string STATE_TITLES = "Quản lý danh mục chức vụ";
        public static string STATE_UNITS = "Quản lý danh mục đơn vị";
        public static string CATE_CRITERIA = "Quản lý tiêu chí";
        public static string BUSINESS_LINE = "Danh mục mặt hàng";
        public static string STAGE = "Danh mục giai đoạn";

        public static string ADMIN_IS_TRATIVE_PRODUCE_FIELD = "Quản lý lĩnh vực giải quyết";
        public static string UNITS = "Quản lý danh mục đơn vị tính";
        public static string RECORDS_FINANCE_PLAN = "Quản lý danh mục nhóm hồ sơ KHTC";
        public static string GAS = "Quản lý danh mục lĩnh vực kinh doanh khí";


        //Phòng thương mại

        //Quản lý cơ sở hạ tầng thương mại
        public static string COMMERCIAL_MANAGEMENT = "Quản lý thông tin chợ, ST, TTTM";
        public static string BUILD_AND_UPGRADE = "Quản lý thông tin chợ, ST, TTTM xây dựng, nâng cấp";
        public static string MARKET_MANAGERMENT = "Quản lý thông tin chợ";
        public static string RURAL_DEVELOPMENT_PLAN = "Kế hoạch phat triển nông thôn";

        //Quản lý xăng dầu, rượu, thuốc lá, đầu tư nước ngoài
        public static string PETROLEUM = "Quản lý đơn vị kinh doanh xăng dầu";
        public static string CIGARETTE = "Quản lý đơn vị bán buôn sản phẩm thuốc lá";
        public static string ALCOHOL = "Quản lý đơn vị bán buôn rượu";
        public static string INTER_COMMERCE = "Quản lý tổ chức kinh tế có vốn đầu tư nước ngoài";

        //Quản lý xuất nhập khẩu 
        public static string EXPORT_GOODS = "Quản lý xuất khẩu";
        public static string IMPORT_GOODS = "Quản lý nhập khẩu";

        //Quản lý bán lẻ
        public static string CATE_RETAIL = "Quản lý tổng mức bán lẻ hàng hóa";
        public static string CONSUMER_SERVICE_REVENUE = "Quản lý doanh thu dịch vụ tiêu dùng";

        public static string NUMBER_SEVEN = "Tiêu chí số 7";

        //Xúc tiến thương mại
        public static string TRADE_PROMOTION_PROJECT_MANAGEMENT = "Quản lý đề án xúc tiến thương mại";
        public static string TRADE_FAIR_ORGANIZATION_CERTIFICATION = "Quản lý xác nhận tham gia hội chợ triển lãm thương mại";
        public static string MANAGE_CONFIRM_PROMOTION = "Quản lý xác nhận chương trình khuyến mãi";

        //Phòng quản lý công nghiệp
        public static string CATE_RP_ANCOL_FOR_FACTORY = "Báo cáo tình hình rượu thủ công bán cho cơ sở có giấy phép sản xuất rượu";
        public static string CATE_RP_CRAFT_ANCOL_FOR_ECONOMIC = "Báo cáo tình hình sản xuất rượu thủ công nhằm mục đích kinh doanh";
        public static string CATE_RP_SOLD_ANCOL = "Báo cáo tình hình bán lẻ rượu";
        public static string CATE_RP_PRODUCE_INDUST_ANCOL = "Báo cáo tình hình sản xuất rượu công nghiệp";
        public static string CATE_RP_TURN_OVER_INDUST_ANCOL = "Báo cáo tình hình kinh doanh rượu công nghiệp";

        public static string CATE_MANAGE_ANCOL_LOCAL_BUSINESS = "Quản lý số liệu doanh nghiệp công ty sản xuất công nghiệp trên địa bàn tỉnh";

        //Quản lý các dự án FDI, DDI
        public static string CATE_PROJECT = "Quản lý danh sách dự án";

        //Quản lý kết quả công tác khuyến nông 
        public static string RESULT_INDUSTRIAL_PROMOTION_VOTING = "Quản lý danh mục báo cáo kết quả bình chọn sản phẩm công nghiệp nông thôn tiêu biểu";
        public static string INDUSTRIAL_PROMOTION_RESULT = "Quản lý danh mục báo cáo kết quả công tác khuyến công";
        public static string INDUSTRIAL_PROMOTION_FUNDING = "Quản lý danh mục báo cáo kinh phí khuyến công";

        //Quản lý thông tin CCN
        public static string INDUSTRIAL_CLUSTER_INFO_MANAGEMENT = "Quản lý dự án đầu tư";
        public static string CATE_INDUSTRIAL_CLUSTER = "Quản lý cụm công nghiệp";
        public static string REPORT_INVESTMENT = "Báo cáo tình hình hoạt động của dự án đầu tư trong cụm công nghiệp";
        public static string REPORT_OF_CONS = "Báo cáo tình hình hoạt động của dự án đầu tư xây dựng hạ tầng kỹ thuật cụm công nghiệp";
        public static string REPORT_INDUSTRIAL_CLUSTER = "Báo cáo tổng hợp tình hình cụm công nghiệp trên địa bàn cấp huyện";

        //Thanh tra sở
        //Thủ tục hành chính
        public static string ADMINISTRATIVE_PROCEDURE = "Thủ tục hành chính";
        public static string PROCESS_ADMINISTRATIVE_PROCEDURE = "Quy trình nội bộ giải quyết thủ tục hành chính";
        public static string REPORT_ADMINISTRATIVE_PROCEDURE = "Báo cáo tình hình giải quyết thủ tục hành chính";

        //Quản lý hoạt động bán hàng đa cấp 
        public static string MULTI_LEVEL_SALE_MANAGEMENT = "Quản lý cơ sở hoạt động bán hàng đa cấp";
        public static string MULTI_LEVEL_SALE_PARTICIPANT = "Quản lý người tham gia bán hàng đa cấp";
        public static string SAMPLE_CONTRACT = "Quản lý hợp đồng mẫu";

        //Trung tâm khuyến công và xúc tiến thương mại
        public static string PARTICIPANT_SUPPORT_FAIR = "Quản lý danh sách tham gia và hỗ trợ hội chợ";
        public static string INDUSTRIAL_PROMOTION_PROJECT = "Quản lý dự án khuyến công";
        public static string REPORT_PROMOTION_COMMERCE = "Quản lý báo cáo hoạt động xúc tiến thương mại";
        public static string TRADE_PROMOTION_ACTIVITY_REPORT = "Quản lý báo cáo hoạt động xúc tiến thương mại";
        public static string PARTICIPANT_TRADE_PROMOTION = "Quản lý doanh nghiệp tham gia chương trình xúc tiến thương mại";
        public static string PRODUCT_OCOP = "Báo cáo sản phẩm đạt OCOP";

        public static string TRAINING_MANAGEMENT = "Quản lý đào tạo tập huấn";
        public static string TRADE_PROMOTION_OTHER = "Quản lý xúc tiến thương mại khác";

        //Phòng kế hoạch tài chính
        //Chỉ số sản xuất công nghiệp tháng

        public static string REPORT_INDEX_INDUSTRY = "Báo cáo chỉ số sản xuất công nghiệp";
        public static string RECORDS_MANAGER = "Quản lý hồ sơ";
        public static string FINANCIAL_PLAN_TARGET = "Các chỉ tiêu sản xuất kinh doanh, xuất khẩu chủ yếu";
        public static string TOTAL_RETAIL_SALE = "Tổng mức bán lẻ hàng hóa";

        //Phòng an toàn kỹ thuật môi trường
        //An toàn thực phẩm ngành công thương
        public static string FOOD_SAFETY_CERTIFICATE = "Quản lý cấp giấy chứng nhận";
        public static string REGULATION_CONFORMITY_ANNOUNCEMENT_MANAGEMENT = "Quản lý công bố hợp quy";
        public static string COMMIT_MANAGER = "Quản lý cam kết: sản xuất, kinh doanh, vừa sản xuất vừa kinh doanh";

        //Bảo vệ môi trường
        public static string TRAINING_CLASS_MANAGEMENT = "Quản lý lớp tập huấn";
        public static string TEST_GUID_MANAGEMENT = "Quản lý kiểm tra hướng dẫn";
        public static string ENVIRONMENT_PROJECT_MANAGEMENT = "Quản lý đề án bảo vệ môi trường";

        //Quản lý an toàn hóa chất
        public static string CHEMICAL_SAFETY_CERTIFICATE = "Quản lý cấp giấy chứng nhận";
        public static string CHEMICAL_BUSINESS_MANAGEMENT = "Quản lý doanh nghiệp hoá chất";

        public static string MANAGEMENT_FIRE_PREVENTION = "Quản lý công tác phòng chống cháy nổ thuộc ngành công thương";
        public static string GAS_BUSINESS = "Quản lý lĩnh vực kinh doanh khí";
        public static string MANAGE_ARCHIVE_RECORDS = "Quản lý hồ sơ lưu trữ";

        //Phòng quản lý năng lượng
        public static string ELECTRICITY_INSPECTOR_CARD = "Quản lý danh sách thẻ kiểm tra viên điện lực";

        //Các dự án nguồn điện
        public static string APPROVED_POWER_PROJECT = "Quản lý dự án được phê duyệt";
        public static string PROPOSED_POWER_PROJECT = "Quản lý dự án nguồn điện đang đề xuất";
        public static string ROOF_TOP_SOLAR_PROJECT_MANAGEMENT = "Quản lý dự án điện mặt trời áp mái";
        public static string COMMUNE_ELECTRICITY_MANAGEMENT = "Quản lý điện cấp xã";

        public static string ELECTRICAL_PROJECT_MANAGEMENT = "Quản lý công trình cao áp trên địa bàn tỉnh";
        public static string LIST_OF_KEY_ENERGY_USERS = "Danh sách cơ sở sử dụng năng lượng trọng điểm";
        public static string MANAGEMENT_ELECTRICITY_ACTIVITIES = "Thông tin quản lý về hoạt động điện lực";
        public static string MANAGEMENT_ELECTRICITY_ACTIVITIES_MONTH_REPORT = "Tài liệu thông tin";
        public static string ELECTRIC_OPERATING_UNIT = "Các đơn vị hoạt động điện lực";

        public static string GAS_TRAINING_CLASS = "Quản lý lớp tập huấn lĩnh vực kinh doanh khí";

        public static string CONFIGS_GROUP = "Cấu hình hệ thống";
        public static string USER = "Danh mục người dùng";
        public static string GROUP_USER = "Quản lý nhóm người dùng";

        //Tiêu chí nông thôn mới
        public static string TARGET4 = "Quản lý điện cấp xã";
        public static string TARGET7 = "Tiêu chí cơ sở hạ tầng thương mại nông thôn";
        public static string TARGET1708 = "An toàn thực phẩm nông thôn mới";
        public static string NEW_RURAL_CRITERIA = "Quản lý tiêu chí nông thôn mới, nông thôn mới nâng cao";
    }
}
