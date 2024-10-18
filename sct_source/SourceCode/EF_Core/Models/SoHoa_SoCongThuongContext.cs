using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EF_Core.Models
{
    public partial class SoHoa_SoCongThuongContext : DbContext
    {
        public SoHoa_SoCongThuongContext()
        {
        }

        public SoHoa_SoCongThuongContext(DbContextOptions<SoHoa_SoCongThuongContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdministrativeFormality> AdministrativeFormalities { get; set; } = null!;
        public virtual DbSet<AdministrativeProcedure> AdministrativeProcedures { get; set; } = null!;
        public virtual DbSet<AlcoholBusiness> AlcoholBusinesses { get; set; } = null!;
        public virtual DbSet<AlcoholBussinessDetail> AlcoholBussinessDetails { get; set; } = null!;
        public virtual DbSet<AlcoholWholesaleLicense> AlcoholWholesaleLicenses { get; set; } = null!;
        public virtual DbSet<ApprovedPowerProject> ApprovedPowerProjects { get; set; } = null!;
        public virtual DbSet<BuildAndUpgradeMarket> BuildAndUpgradeMarkets { get; set; } = null!;
        public virtual DbSet<Business> Businesses { get; set; } = null!;
        public virtual DbSet<BusinessIndustry> BusinessIndustries { get; set; } = null!;
        public virtual DbSet<BusinessLine> BusinessLines { get; set; } = null!;
        public virtual DbSet<BusinessLogo> BusinessLogos { get; set; } = null!;
        public virtual DbSet<BusinessMultiLevel> BusinessMultiLevels { get; set; } = null!;
        public virtual DbSet<BusinessMultiLevelNumCert> BusinessMultiLevelNumCerts { get; set; } = null!;
        public virtual DbSet<CateCriteriaNumberSeven> CateCriteriaNumberSevens { get; set; } = null!;
        public virtual DbSet<CateCriteriaNumberSevenDetail> CateCriteriaNumberSevenDetails { get; set; } = null!;
        public virtual DbSet<CateCriterion> CateCriteria { get; set; } = null!;
        public virtual DbSet<CateIndustrialCluster> CateIndustrialClusters { get; set; } = null!;
        public virtual DbSet<CateIntegratedManagement> CateIntegratedManagements { get; set; } = null!;
        public virtual DbSet<CateIntegratedManagementDisbursement> CateIntegratedManagementDisbursements { get; set; } = null!;
        public virtual DbSet<CateIntegratedManagementHistory> CateIntegratedManagementHistories { get; set; } = null!;
        public virtual DbSet<CateInvestmentProject> CateInvestmentProjects { get; set; } = null!;
        public virtual DbSet<CateManageAncolLocalBussine> CateManageAncolLocalBussines { get; set; } = null!;
        public virtual DbSet<CateManageAncolLocalBussinesDetail> CateManageAncolLocalBussinesDetails { get; set; } = null!;
        public virtual DbSet<CateManageAncolLocalBussinesTypeOfProfession> CateManageAncolLocalBussinesTypeOfProfessions { get; set; } = null!;
        public virtual DbSet<CateProject> CateProjects { get; set; } = null!;
        public virtual DbSet<CateProjectDisbursement> CateProjectDisbursements { get; set; } = null!;
        public virtual DbSet<CateProjectHistory> CateProjectHistories { get; set; } = null!;
        public virtual DbSet<CateReportProduceCrafttAncolForEconomic> CateReportProduceCrafttAncolForEconomics { get; set; } = null!;
        public virtual DbSet<CateReportProduceIndustlAncol> CateReportProduceIndustlAncols { get; set; } = null!;
        public virtual DbSet<CateReportSoldAncol> CateReportSoldAncols { get; set; } = null!;
        public virtual DbSet<CateReportSoldAncolForFactoryLicense> CateReportSoldAncolForFactoryLicenses { get; set; } = null!;
        public virtual DbSet<CateReportTurnOverIndustAncol> CateReportTurnOverIndustAncols { get; set; } = null!;
        public virtual DbSet<CateRetail> CateRetails { get; set; } = null!;
        public virtual DbSet<CateRetailDetail> CateRetailDetails { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<CategoryType> CategoryTypes { get; set; } = null!;
        public virtual DbSet<ChemicalBusinessManagement> ChemicalBusinessManagements { get; set; } = null!;
        public virtual DbSet<ChemicalSafetyCertificate> ChemicalSafetyCertificates { get; set; } = null!;
        public virtual DbSet<ChemicalSafetyCertificateAttachFile> ChemicalSafetyCertificateAttachFiles { get; set; } = null!;
        public virtual DbSet<ChemicalSafetyCertificateChemicalInfo> ChemicalSafetyCertificateChemicalInfos { get; set; } = null!;
        public virtual DbSet<CigaretteBusiness> CigaretteBusinesses { get; set; } = null!;
        public virtual DbSet<CigaretteBusinessStore> CigaretteBusinessStores { get; set; } = null!;
        public virtual DbSet<CommercialManagement> CommercialManagements { get; set; } = null!;
        public virtual DbSet<CommitManager> CommitManagers { get; set; } = null!;
        public virtual DbSet<CommitManagerListItem> CommitManagerListItems { get; set; } = null!;
        public virtual DbSet<Commune> Communes { get; set; } = null!;
        public virtual DbSet<CommuneElectricityManagement> CommuneElectricityManagements { get; set; } = null!;
        public virtual DbSet<ConsumerServiceRevenue> ConsumerServiceRevenues { get; set; } = null!;
        public virtual DbSet<ConsumerServiceRevenueDetail> ConsumerServiceRevenueDetails { get; set; } = null!;
        public virtual DbSet<Coordinate> Coordinates { get; set; } = null!;
        public virtual DbSet<Country> Countries { get; set; } = null!;
        public virtual DbSet<Departmemt> Departmemts { get; set; } = null!;
        public virtual DbSet<District> Districts { get; set; } = null!;
        public virtual DbSet<DistrictCustom> DistrictCustoms { get; set; } = null!;
        public virtual DbSet<ElectricOperatingUnit> ElectricOperatingUnits { get; set; } = null!;
        public virtual DbSet<ElectricalProjectManagement> ElectricalProjectManagements { get; set; } = null!;
        public virtual DbSet<ElectricityInspectorCard> ElectricityInspectorCards { get; set; } = null!;
        public virtual DbSet<EnergyIndustry> EnergyIndustries { get; set; } = null!;
        public virtual DbSet<EnvironmentProjectManagement> EnvironmentProjectManagements { get; set; } = null!;
        public virtual DbSet<EnvironmentProjectManagementAttachFile> EnvironmentProjectManagementAttachFiles { get; set; } = null!;
        public virtual DbSet<ExportGood> ExportGoods { get; set; } = null!;
        public virtual DbSet<FinancialPlanTarget> FinancialPlanTargets { get; set; } = null!;
        public virtual DbSet<FoodSafetyCertificate> FoodSafetyCertificates { get; set; } = null!;
        public virtual DbSet<FoodSafetyCertificateAttachFile> FoodSafetyCertificateAttachFiles { get; set; } = null!;
        public virtual DbSet<FoodSafetyCertificateItem> FoodSafetyCertificateItems { get; set; } = null!;
        public virtual DbSet<Ga> Gas { get; set; } = null!;
        public virtual DbSet<GasBusiness> GasBusinesses { get; set; } = null!;
        public virtual DbSet<GasTrainingClassManagement> GasTrainingClassManagements { get; set; } = null!;
        public virtual DbSet<GasTrainingClassManagementAttachFile> GasTrainingClassManagementAttachFiles { get; set; } = null!;
        public virtual DbSet<Group> Groups { get; set; } = null!;
        public virtual DbSet<GroupPermit> GroupPermits { get; set; } = null!;
        public virtual DbSet<ImportDonvi> ImportDonvis { get; set; } = null!;
        public virtual DbSet<ImportGood> ImportGoods { get; set; } = null!;
        public virtual DbSet<ImportLoaihinhXdnc> ImportLoaihinhXdncs { get; set; } = null!;
        public virtual DbSet<ImportTemp> ImportTemps { get; set; } = null!;
        public virtual DbSet<IndustrialManagementTarget> IndustrialManagementTargets { get; set; } = null!;
        public virtual DbSet<IndustrialManagementTargetChild> IndustrialManagementTargetChildren { get; set; } = null!;
        public virtual DbSet<IndustrialPromotionFundingReport> IndustrialPromotionFundingReports { get; set; } = null!;
        public virtual DbSet<IndustrialPromotionProject> IndustrialPromotionProjects { get; set; } = null!;
        public virtual DbSet<IndustrialPromotionProjectDetail> IndustrialPromotionProjectDetails { get; set; } = null!;
        public virtual DbSet<IndustrialPromotionResultsReport> IndustrialPromotionResultsReports { get; set; } = null!;
        public virtual DbSet<Industry> Industries { get; set; } = null!;
        public virtual DbSet<InternationalCommerce> InternationalCommerces { get; set; } = null!;
        public virtual DbSet<ListOfKeyEnergyUser> ListOfKeyEnergyUsers { get; set; } = null!;
        public virtual DbSet<LoginSession> LoginSessions { get; set; } = null!;
        public virtual DbSet<MainMenu> MainMenus { get; set; } = null!;
        public virtual DbSet<ManageArchiveRecord> ManageArchiveRecords { get; set; } = null!;
        public virtual DbSet<ManageConfirmPromotion> ManageConfirmPromotions { get; set; } = null!;
        public virtual DbSet<ManageConfirmPromotionAttachFile> ManageConfirmPromotionAttachFiles { get; set; } = null!;
        public virtual DbSet<ManagementElectricityActivity> ManagementElectricityActivities { get; set; } = null!;
        public virtual DbSet<ManagementFirePrevention> ManagementFirePreventions { get; set; } = null!;
        public virtual DbSet<ManagementSeminar> ManagementSeminars { get; set; } = null!;
        public virtual DbSet<MarketDevelopPlan> MarketDevelopPlans { get; set; } = null!;
        public virtual DbSet<MarketInvestEnterprise> MarketInvestEnterprises { get; set; } = null!;
        public virtual DbSet<MarketManagement> MarketManagements { get; set; } = null!;
        public virtual DbSet<MarketManagementDetail> MarketManagementDetails { get; set; } = null!;
        public virtual DbSet<MarketPlanInformation> MarketPlanInformations { get; set; } = null!;
        public virtual DbSet<MarketTargetSeven> MarketTargetSevens { get; set; } = null!;
        public virtual DbSet<MeaMonthReport> MeaMonthReports { get; set; } = null!;
        public virtual DbSet<MeaMonthReportAttachFile> MeaMonthReportAttachFiles { get; set; } = null!;
        public virtual DbSet<Module> Modules { get; set; } = null!;
        public virtual DbSet<MultiLevelSalesManagement> MultiLevelSalesManagements { get; set; } = null!;
        public virtual DbSet<MultiLevelSalesParticipant> MultiLevelSalesParticipants { get; set; } = null!;
        public virtual DbSet<NewRuralCriterion> NewRuralCriteria { get; set; } = null!;
        public virtual DbSet<ParticipateSupportFair> ParticipateSupportFairs { get; set; } = null!;
        public virtual DbSet<ParticipateSupportFairDetail> ParticipateSupportFairDetails { get; set; } = null!;
        public virtual DbSet<Permission> Permissions { get; set; } = null!;
        public virtual DbSet<PetroleumBusiness> PetroleumBusinesses { get; set; } = null!;
        public virtual DbSet<PetroleumBusinessStore> PetroleumBusinessStores { get; set; } = null!;
        public virtual DbSet<Position> Positions { get; set; } = null!;
        public virtual DbSet<ProcessAdministrativeProcedure> ProcessAdministrativeProcedures { get; set; } = null!;
        public virtual DbSet<ProcessAdministrativeProceduresStep> ProcessAdministrativeProceduresSteps { get; set; } = null!;
        public virtual DbSet<ProductOcop> ProductOcops { get; set; } = null!;
        public virtual DbSet<ProductOcopAttachFile> ProductOcopAttachFiles { get; set; } = null!;
        public virtual DbSet<ProposedPowerProject> ProposedPowerProjects { get; set; } = null!;
        public virtual DbSet<RecordsFinancePlan> RecordsFinancePlans { get; set; } = null!;
        public virtual DbSet<RecordsManager> RecordsManagers { get; set; } = null!;
        public virtual DbSet<RegulationConformityAm> RegulationConformityAms { get; set; } = null!;
        public virtual DbSet<RegulationConformityAmLog> RegulationConformityAmLogs { get; set; } = null!;
        public virtual DbSet<RegulationConformityAmProduct> RegulationConformityAmProducts { get; set; } = null!;
        public virtual DbSet<ReportAdministrativeProcedure> ReportAdministrativeProcedures { get; set; } = null!;
        public virtual DbSet<ReportIndexIndustry> ReportIndexIndustries { get; set; } = null!;
        public virtual DbSet<ReportIndustrialCluster> ReportIndustrialClusters { get; set; } = null!;
        public virtual DbSet<ReportOperationalStatusOfConstructionInvestmentProject> ReportOperationalStatusOfConstructionInvestmentProjects { get; set; } = null!;
        public virtual DbSet<ReportOperationalStatusOfInvestmentProject> ReportOperationalStatusOfInvestmentProjects { get; set; } = null!;
        public virtual DbSet<ReportPromotionCommerce> ReportPromotionCommerces { get; set; } = null!;
        public virtual DbSet<ResultsIndustrialPromotionVotingRp> ResultsIndustrialPromotionVotingRps { get; set; } = null!;
        public virtual DbSet<RooftopSolarProjectManagement> RooftopSolarProjectManagements { get; set; } = null!;
        public virtual DbSet<RuralDevelopmentPlan> RuralDevelopmentPlans { get; set; } = null!;
        public virtual DbSet<RuralDevelopmentPlanStage> RuralDevelopmentPlanStages { get; set; } = null!;
        public virtual DbSet<Sample> Samples { get; set; } = null!;
        public virtual DbSet<SampleContract> SampleContracts { get; set; } = null!;
        public virtual DbSet<Stage> Stages { get; set; } = null!;
        public virtual DbSet<StateTitle> StateTitles { get; set; } = null!;
        public virtual DbSet<StateUnit> StateUnits { get; set; } = null!;
        public virtual DbSet<SubMenu> SubMenus { get; set; } = null!;
        public virtual DbSet<SysColumn> SysColumns { get; set; } = null!;
        public virtual DbSet<SysTable> SysTables { get; set; } = null!;
        public virtual DbSet<SystemLog> SystemLogs { get; set; } = null!;
        public virtual DbSet<TableResult> TableResults { get; set; } = null!;
        public virtual DbSet<Target1708> Target1708s { get; set; } = null!;
        public virtual DbSet<Target7> Target7s { get; set; } = null!;
        public virtual DbSet<TblTest> TblTests { get; set; } = null!;
        public virtual DbSet<TestGuidManagement> TestGuidManagements { get; set; } = null!;
        public virtual DbSet<TestGuidManagementAttachFile> TestGuidManagementAttachFiles { get; set; } = null!;
        public virtual DbSet<TimeManagementSeminar> TimeManagementSeminars { get; set; } = null!;
        public virtual DbSet<TotalRetailSale> TotalRetailSales { get; set; } = null!;
        public virtual DbSet<TradeFairOrganizationCertification> TradeFairOrganizationCertifications { get; set; } = null!;
        public virtual DbSet<TradeFairOrganizationCertificationAttachFile> TradeFairOrganizationCertificationAttachFiles { get; set; } = null!;
        public virtual DbSet<TradeFairOrganizationCertificationTime> TradeFairOrganizationCertificationTimes { get; set; } = null!;
        public virtual DbSet<TradePromotionActivityReport> TradePromotionActivityReports { get; set; } = null!;
        public virtual DbSet<TradePromotionActivityReportAttachFile> TradePromotionActivityReportAttachFiles { get; set; } = null!;
        public virtual DbSet<TradePromotionActivityReportParticipatingBusiness> TradePromotionActivityReportParticipatingBusinesses { get; set; } = null!;
        public virtual DbSet<TradePromotionOther> TradePromotionOthers { get; set; } = null!;
        public virtual DbSet<TradePromotionOtherAttachFile> TradePromotionOtherAttachFiles { get; set; } = null!;
        public virtual DbSet<TradePromotionProgramForBusiness> TradePromotionProgramForBusinesses { get; set; } = null!;
        public virtual DbSet<TradePromotionProgramForBusinessDetail> TradePromotionProgramForBusinessDetails { get; set; } = null!;
        public virtual DbSet<TradePromotionProject> TradePromotionProjects { get; set; } = null!;
        public virtual DbSet<TradePromotionProjectManagement> TradePromotionProjectManagements { get; set; } = null!;
        public virtual DbSet<TradePromotionProjectManagementAttachFile> TradePromotionProjectManagementAttachFiles { get; set; } = null!;
        public virtual DbSet<TradePromotionProjectManagementBussiness> TradePromotionProjectManagementBussinesses { get; set; } = null!;
        public virtual DbSet<TrainingClassManagement> TrainingClassManagements { get; set; } = null!;
        public virtual DbSet<TrainingClassManagementAttachFile> TrainingClassManagementAttachFiles { get; set; } = null!;
        public virtual DbSet<TrainingManagement> TrainingManagements { get; set; } = null!;
        public virtual DbSet<TrainingManagementAttachFile> TrainingManagementAttachFiles { get; set; } = null!;
        public virtual DbSet<TypeOfBusiness> TypeOfBusinesses { get; set; } = null!;
        public virtual DbSet<TypeOfEnergy> TypeOfEnergies { get; set; } = null!;
        public virtual DbSet<TypeOfMarket> TypeOfMarkets { get; set; } = null!;
        public virtual DbSet<TypeOfProfession> TypeOfProfessions { get; set; } = null!;
        public virtual DbSet<TypeOfTradePromotion> TypeOfTradePromotions { get; set; } = null!;
        public virtual DbSet<Unit> Units { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserGroup> UserGroups { get; set; } = null!;
        public virtual DbSet<UuiRefreshToken> UuiRefreshTokens { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=125.212.216.115,11443;Initial Catalog=SoHoa_SoCongThuong;User ID=SoHoa_SoCongThuong;Password=vIdGiPd*L3D3ev$QbiNovheF");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdministrativeFormality>(entity =>
            {
                entity.HasKey(e => e.AdminFormalitiesId);

                entity.Property(e => e.AdminFormalitiesId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AdminFormalitiesCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.AdminFormalitiesName).HasMaxLength(500);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DocUrl).HasComment("Url của Sở");

                entity.Property(e => e.Dvclevel)
                    .HasColumnName("DVCLevel")
                    .HasComment("Cấp DVC: 1 - Toàn trình, 2 - Còn lại");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<AdministrativeProcedure>(entity =>
            {
                entity.HasKey(e => e.AdministrativeProceduresId);

                entity.Property(e => e.AdministrativeProceduresId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng thủ tục hành chính - Sở thanh tra");

                entity.Property(e => e.AdministrativeProceduresCode)
                    .HasMaxLength(100)
                    .HasComment("Mã thủ tục");

                entity.Property(e => e.AdministrativeProceduresField).HasComment("Lĩnh vực giải quyết");

                entity.Property(e => e.AdministrativeProceduresName).HasMaxLength(500);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DayReception).HasColumnType("datetime");

                entity.Property(e => e.FinishDay).HasColumnType("datetime");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ReceptionForm).HasComment("Hình thức tiếp nhận - 1: Trực tiếp, 2: Tiếp tuyến");

                entity.Property(e => e.SettlementTerm).HasColumnType("datetime");

                entity.Property(e => e.Status).HasComment("Trạng thái - 1: Chưa xử lý, 2: Đang xử lý, 3: Đã xử lý");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<AlcoholBusiness>(entity =>
            {
                entity.ToTable("Alcohol_Business");

                entity.Property(e => e.AlcoholBusinessId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.PhoneNumber).HasMaxLength(11);

                entity.Property(e => e.Representative).HasMaxLength(200);

                entity.Property(e => e.Supplier).HasMaxLength(200);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<AlcoholBussinessDetail>(entity =>
            {
                entity.HasKey(e => e.AlcoholBusinessDetailId);

                entity.ToTable("Alcohol_Bussiness_Detail");

                entity.Property(e => e.AlcoholBusinessDetailId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DiaChi).HasMaxLength(200);

                entity.Property(e => e.DiaChiDonViCungCap).HasMaxLength(200);

                entity.Property(e => e.DonViCungCap).HasMaxLength(200);

                entity.Property(e => e.GhiChu).HasMaxLength(200);

                entity.Property(e => e.GiayPhepKinhDoanh).HasMaxLength(50);

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.NgayCapGiayPhepBanLe).HasColumnType("datetime");

                entity.Property(e => e.NgayHetHan).HasColumnType("datetime");

                entity.Property(e => e.NguoiDaiDien).HasMaxLength(200);

                entity.Property(e => e.SoDienThoai).HasMaxLength(20);

                entity.Property(e => e.SoDienThoaiDonViCungCap).HasMaxLength(20);

                entity.Property(e => e.TenDoanhNghiep).HasMaxLength(200);
            });

            modelBuilder.Entity<AlcoholWholesaleLicense>(entity =>
            {
                entity.ToTable("AlcoholWholesaleLicense");

                entity.Property(e => e.AlcoholWholesaleLicenseId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.ExpirationDate).HasColumnType("datetime");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.LicenseDate).HasColumnType("datetime");

                entity.Property(e => e.LicenseNumber).HasMaxLength(50);

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("datetime")
                    .HasComment("Thời gian chỉnh sửa");

                entity.Property(e => e.UpdateUserId).HasComment("Người chỉnh sửa");
            });

            modelBuilder.Entity<ApprovedPowerProject>(entity =>
            {
                entity.ToTable("ApprovedPowerProject");

                entity.Property(e => e.ApprovedPowerProjectId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Quản lý dự án nguồn điện được phê duyệt");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ");

                entity.Property(e => e.Area)
                    .HasColumnType("decimal(18, 2)")
                    .HasComment("Diện tích");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DistrictId).HasComment("Huyện");

                entity.Property(e => e.InvestorName)
                    .HasMaxLength(500)
                    .HasComment("Nhà đầu tư");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note)
                    .HasMaxLength(500)
                    .HasComment("Ghi chú");

                entity.Property(e => e.PolicyDecision)
                    .HasMaxLength(500)
                    .HasComment("Quyết định chủ trương");

                entity.Property(e => e.PowerOutput).HasComment("Sản lượng điện phát");

                entity.Property(e => e.ProjectName)
                    .HasMaxLength(500)
                    .HasComment("Tên dự án");

                entity.Property(e => e.Substation).HasComment("Trạm biến áp");

                entity.Property(e => e.Turbines).HasComment("Số lượng tuabin");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Wattage).HasComment("Công suất");
            });

            modelBuilder.Entity<BuildAndUpgradeMarket>(entity =>
            {
                entity.HasKey(e => e.BuildAndUpgradeId);

                entity.ToTable("BuildAndUpgradeMarket");

                entity.Property(e => e.BuildAndUpgradeId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.AnotherCapital).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.AnotherCapitalUnit).HasMaxLength(50);

                entity.Property(e => e.BudgetCapital).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.BudgetCapitalUnit).HasMaxLength(50);

                entity.Property(e => e.BuildAndUpgradeName).HasMaxLength(100);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsBuild).HasComment("1: True 0: False");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.IsUpgrade)
                    .HasColumnName("isUpgrade")
                    .HasComment("1: True 0: False");

                entity.Property(e => e.LandUseCapital).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.LandUseCapitalUnit).HasMaxLength(50);

                entity.Property(e => e.Loans).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.LoansUnit).HasMaxLength(50);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.RealizedCapital).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.RealizedCapitalUnit).HasMaxLength(50);

                entity.Property(e => e.TotalInvestment).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.TotalInvestmentUnit).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Business>(entity =>
            {
                entity.ToTable("Business");

                entity.Property(e => e.BusinessId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BusinessCode).HasMaxLength(100);

                entity.Property(e => e.BusinessNameEn).HasMaxLength(500);

                entity.Property(e => e.BusinessNameVi).HasMaxLength(500);

                entity.Property(e => e.Cccd)
                    .HasMaxLength(12)
                    .HasColumnName("CCCD");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DiaChi).HasMaxLength(500);

                entity.Property(e => e.DiaChiTruSo).HasMaxLength(500);

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.GiamDoc).HasMaxLength(500);

                entity.Property(e => e.GiayPhepSanXuat).HasMaxLength(100);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.MaSoThue).HasMaxLength(50);

                entity.Property(e => e.NgayCap).HasColumnType("date");

                entity.Property(e => e.NgayCapPhep).HasColumnType("date");

                entity.Property(e => e.NgayHoatDong).HasColumnType("date");

                entity.Property(e => e.NgaySinh).HasColumnType("date");

                entity.Property(e => e.NguoiDaiDien).HasMaxLength(500);

                entity.Property(e => e.NoiCap).HasMaxLength(500);

                entity.Property(e => e.SoDienThoai).HasMaxLength(100);

                entity.Property(e => e.TenGiaoDich).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<BusinessIndustry>(entity =>
            {
                entity.ToTable("Business_Industry");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<BusinessLine>(entity =>
            {
                entity.ToTable("BusinessLine");

                entity.Property(e => e.BusinessLineId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BusinessLineCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.BusinessLineName).HasMaxLength(500);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<BusinessLogo>(entity =>
            {
                entity.HasKey(e => e.LogoId);

                entity.ToTable("Business_Logo");

                entity.Property(e => e.LogoId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("File đính kèm văn bản xác nhận - Quản lý xác nhận tổ chức hội chợ triển lãm thương mại - Xúc tiến thương mại");

                entity.Property(e => e.LinkFile).HasMaxLength(250);
            });

            modelBuilder.Entity<BusinessMultiLevel>(entity =>
            {
                entity.ToTable("BusinessMultiLevel");

                entity.Property(e => e.BusinessMultiLevelId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.AddressContact).HasMaxLength(200);

                entity.Property(e => e.CertDate).HasColumnType("datetime");

                entity.Property(e => e.CertExp).HasColumnType("datetime");

                entity.Property(e => e.Contact).HasMaxLength(50);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Goods).HasMaxLength(500);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.LocalConfirm).HasMaxLength(200);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.NumCert).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<BusinessMultiLevelNumCert>(entity =>
            {
                entity.ToTable("BusinessMultiLevelNumCert");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.NumCert).HasMaxLength(200);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CateCriteriaNumberSeven>(entity =>
            {
                entity.ToTable("CateCriteriaNumberSeven");

                entity.Property(e => e.CateCriteriaNumberSevenId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CateCriteriaNumberSevenCode).HasMaxLength(100);

                entity.Property(e => e.ConfirmTime).HasColumnType("datetime");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Importtime)
                    .HasColumnType("datetime")
                    .HasColumnName("importtime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ReportMonth).HasMaxLength(100);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CateCriteriaNumberSevenDetail>(entity =>
            {
                entity.ToTable("CateCriteriaNumberSevenDetail");

                entity.Property(e => e.CateCriteriaNumberSevenDetailId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.NumberOfQualifyingWard).HasComment("số xã đạt tiêu chuẩn tiêu chí số 7");

                entity.Property(e => e.NumberOfWard).HasComment("số lượng xã trong huyện");

                entity.Property(e => e.NumberOfWardCommercialInfrastructure).HasComment("số lượng xã đạt tiêu chuẩn hạ tầng thương mại ");

                entity.Property(e => e.NumberOfWardCommercialInfrastructureEstimate)
                    .HasColumnName("NumberOfWardCommercialInfrastructure_Estimate")
                    .HasComment("số lượng xã đạt tiêu chuẩn hạ tầng thương mại ");

                entity.Property(e => e.NumberOfWardCommercialInfrastructurePlan)
                    .HasColumnName("NumberOfWardCommercialInfrastructure_Plan")
                    .HasComment("số lượng xã đạt tiêu chuẩn hạ tầng thương mại ");

                entity.Property(e => e.NumberOfWardNewCountryside).HasComment("số xã đạt nông thôn mới");

                entity.Property(e => e.NumberOfWardNewCountrysideEstimate)
                    .HasColumnName("NumberOfWardNewCountryside_Estimate")
                    .HasComment("số xã đạt nông thôn mới");

                entity.Property(e => e.NumberOfWardNewCountrysidePlan)
                    .HasColumnName("NumberOfWardNewCountryside_Plan")
                    .HasComment("số xã đạt nông thôn mới");
            });

            modelBuilder.Entity<CateCriterion>(entity =>
            {
                entity.HasKey(e => e.CateCriteriaId);

                entity.Property(e => e.CateCriteriaId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng danh mục tiêu chí");

                entity.Property(e => e.CateCriteriaName).HasMaxLength(500);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CateIndustrialCluster>(entity =>
            {
                entity.HasKey(e => e.CateIndustrialClustersId);

                entity.Property(e => e.CateIndustrialClustersId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ApprovalDecision)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("quyết định phê duyệt");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DecisionExpandCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("Quyết định mở rộng");

                entity.Property(e => e.DetailedArea).HasComment("Diện tích chi tiết");

                entity.Property(e => e.EstablishCode)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')")
                    .HasComment("Quyết định thành lập");

                entity.Property(e => e.ExpandedArea).HasComment("Diện tích mở rộng");

                entity.Property(e => e.IndustrialArea).HasComment("Diện tích đất công nghiệp");

                entity.Property(e => e.IndustrialClustersName)
                    .HasMaxLength(250)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.Occupancy).HasComment("Tỷ lệ lấp đầy");

                entity.Property(e => e.RentedArea).HasComment("Diện tích đã cho thuê");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CateIntegratedManagement>(entity =>
            {
                entity.HasKey(e => e.IntegratedManagementId);

                entity.ToTable("CateIntegratedManagement");

                entity.Property(e => e.IntegratedManagementId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.Area).HasComment("1: Trong nước, 2: ngoài nước");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.InvestmentCertificateCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.InvestmentCertificateDate).HasColumnType("datetime");

                entity.Property(e => e.Investors).HasMaxLength(250);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.PolicyDecisions).HasMaxLength(250);

                entity.Property(e => e.PolicyDecisionsDate).HasColumnType("datetime");

                entity.Property(e => e.ProjectAddress)
                    .HasMaxLength(250)
                    .HasComment("Địa điểm thực hiện dự án");

                entity.Property(e => e.ProjectImplementationScale)
                    .HasMaxLength(250)
                    .HasComment("mục tiêu, quy mô thực hiện dự án");

                entity.Property(e => e.ProjectLegalRepresent)
                    .HasMaxLength(250)
                    .HasComment("người đại diện pháp luật");

                entity.Property(e => e.ProjectLicenseYear).HasComment("năm cấp phép");

                entity.Property(e => e.ProjectLocalArea)
                    .HasMaxLength(250)
                    .HasComment("địa bàn");

                entity.Property(e => e.ProjectName).HasMaxLength(250);

                entity.Property(e => e.ProjectOperatingTime).HasComment("thời gian thực hiện (năm)");

                entity.Property(e => e.ProjectPhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasComment("số diện thoại liên lạc");

                entity.Property(e => e.ProjectProgress)
                    .HasMaxLength(250)
                    .HasComment("tiến độ thực hiện dự án, tiến độ đã đăng ký");

                entity.Property(e => e.ProjectProgressActual)
                    .HasMaxLength(250)
                    .HasComment("tiến độ thực tế");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CateIntegratedManagementDisbursement>(entity =>
            {
                entity.ToTable("CateIntegratedManagement_Disbursement");

                entity.Property(e => e.CateIntegratedManagementDisbursementId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DisbursementDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CateIntegratedManagementHistory>(entity =>
            {
                entity.ToTable("CateIntegratedManagement_History");

                entity.Property(e => e.CateIntegratedManagementHistoryId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ContentAdjust).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CateInvestmentProject>(entity =>
            {
                entity.ToTable("CateInvestmentProject");

                entity.Property(e => e.CateInvestmentProjectId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BusinessName).HasMaxLength(250);

                entity.Property(e => e.Career).HasMaxLength(100);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.InvestmentType).HasComment("1: trong nước, 2 ngoài nước");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Owner).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.Produce).HasComment("năng suất sản xuất");

                entity.Property(e => e.ProjectArea).HasComment("Diện tích dự án");

                entity.Property(e => e.Quantity).HasComment("Sản lượng 1 ngày");

                entity.Property(e => e.Reality)
                    .HasMaxLength(250)
                    .HasColumnName("reality");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CateManageAncolLocalBussine>(entity =>
            {
                entity.HasKey(e => e.CateManageAncolLocalBussinessId);

                entity.Property(e => e.CateManageAncolLocalBussinessId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DateChange).HasColumnType("datetime");

                entity.Property(e => e.DateRelease).HasColumnType("datetime");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsActive)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.TypeOfProfessionId).HasComment("lấy từ bảng TypeOfProfession, một loại ngành nghề chính");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CateManageAncolLocalBussinesDetail>(entity =>
            {
                entity.ToTable("CateManageAncolLocalBussines_Detail");

                entity.Property(e => e.CateManageAncolLocalBussinesDetailId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Fullname).HasMaxLength(250);

                entity.Property(e => e.Type).HasComment("1: thành viên góp vốn\r\n2: cổ đông");
            });

            modelBuilder.Entity<CateManageAncolLocalBussinesTypeOfProfession>(entity =>
            {
                entity.HasKey(e => e.CateManageAncolLocalBussinesTypeProfessionId);

                entity.ToTable("CateManageAncolLocalBussines_TypeOfProfession");

                entity.Property(e => e.CateManageAncolLocalBussinesTypeProfessionId).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<CateProject>(entity =>
            {
                entity.ToTable("CateProject");

                entity.Property(e => e.CateProjectId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ActualPurchase).HasComment("mua thực tế");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.Area).HasComment("1: Trong nước, 2: ngoài nước");

                entity.Property(e => e.CapitalContributionTradingTime).HasColumnType("datetime");

                entity.Property(e => e.CapitalPurchase).HasComment("vốn mua");

                entity.Property(e => e.CharterCapitalAfterPurchase).HasComment("vốn điều lệ sau khi mua");

                entity.Property(e => e.CompanyBuy)
                    .HasMaxLength(250)
                    .HasDefaultValueSql("('')")
                    .HasComment("công ty bán");

                entity.Property(e => e.CompanySell).HasComment("Công ty bán/công ty nhận, lấy từ danh mục doanh nghiệp");

                entity.Property(e => e.Country).HasComment("lấy từ danh mục quốc gia");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.InitialCharterCapital).HasComment("vốn điều lệ ban đầu");

                entity.Property(e => e.InvestmentCertificateCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.InvestmentCertificateDate).HasColumnType("datetime");

                entity.Property(e => e.Investors).HasMaxLength(250);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.PolicyDecisions).HasMaxLength(250);

                entity.Property(e => e.PolicyDecisionsDate).HasColumnType("datetime");

                entity.Property(e => e.Profession)
                    .HasMaxLength(250)
                    .HasComment("ngành nghề");

                entity.Property(e => e.ProjectAddress)
                    .HasMaxLength(250)
                    .HasComment("Địa điểm thực hiện dự án");

                entity.Property(e => e.ProjectDecisionToWithdraw)
                    .HasMaxLength(250)
                    .HasDefaultValueSql("('')")
                    .HasComment("quyết định thu hồi");

                entity.Property(e => e.ProjectDecisionToWithdrawDate).HasColumnType("datetime");

                entity.Property(e => e.ProjectFdi)
                    .HasMaxLength(250)
                    .HasColumnName("ProjectFDI")
                    .HasDefaultValueSql("('')")
                    .HasComment("FDI");

                entity.Property(e => e.ProjectId).HasComment("lấy từ chính bảng Cate Project loại 1,2");

                entity.Property(e => e.ProjectImplementationScale)
                    .HasMaxLength(250)
                    .HasDefaultValueSql("('')")
                    .HasComment("mục tiêu, quy mô thực hiện dự án");

                entity.Property(e => e.ProjectImplementationYear).HasComment("Năm thực hiện");

                entity.Property(e => e.ProjectInvestmentForm)
                    .HasMaxLength(250)
                    .HasDefaultValueSql("('')")
                    .HasComment("hình thức đầu tư");

                entity.Property(e => e.ProjectLegalRepresent)
                    .HasMaxLength(250)
                    .HasComment("người đại diện pháp luật");

                entity.Property(e => e.ProjectLicenseYear).HasComment("năm cấp phép");

                entity.Property(e => e.ProjectLocalArea)
                    .HasMaxLength(250)
                    .HasComment("địa bàn");

                entity.Property(e => e.ProjectName).HasMaxLength(250);

                entity.Property(e => e.ProjectOperatingTime).HasComment("thời gian thực hiện (năm)");

                entity.Property(e => e.ProjectPartnerNationality)
                    .HasMaxLength(250)
                    .HasDefaultValueSql("('')")
                    .HasComment("quốc tịch/đối tác");

                entity.Property(e => e.ProjectPhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasComment("số diện thoại liên lạc");

                entity.Property(e => e.ProjectProgress)
                    .HasMaxLength(250)
                    .HasComment("tiến độ thực hiện dự án, tiến độ đã đăng ký");

                entity.Property(e => e.ProjectProgressActual)
                    .HasMaxLength(250)
                    .HasComment("tiến độ thực tế");

                entity.Property(e => e.Units).HasComment("lấy từ bảng units");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CateProjectDisbursement>(entity =>
            {
                entity.ToTable("CateProject_Disbursement");

                entity.Property(e => e.CateProjectDisbursementId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DisbursementDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<CateProjectHistory>(entity =>
            {
                entity.ToTable("CateProject_History");

                entity.Property(e => e.CateProjectHistoryId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ContentAdjust).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CateReportProduceCrafttAncolForEconomic>(entity =>
            {
                entity.ToTable("CateReportProduceCrafttAncolForEconomic");

                entity.Property(e => e.CateReportProduceCrafttAncolForEconomicId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Quantity).HasComment("Sản lượng");

                entity.Property(e => e.QuantityConsume).HasComment("Sản lượng tiêu thụ");

                entity.Property(e => e.TypeofWine)
                    .HasMaxLength(250)
                    .HasComment("chủng loại rượu");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.YearReport).HasComment("Năm báo cáo");
            });

            modelBuilder.Entity<CateReportProduceIndustlAncol>(entity =>
            {
                entity.ToTable("CateReportProduceIndustlAncol");

                entity.Property(e => e.CateReportProduceIndustlAncolId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BusinessId).HasComment("Id doanh nghiệp");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DesignCapacity)
                    .HasMaxLength(250)
                    .HasComment("Công suất thiết kế");

                entity.Property(e => e.Investment).HasComment("Vốn đầu tư");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.QuantityConsume).HasComment("Sản lượng tiêu thụ");

                entity.Property(e => e.QuantityProduction).HasComment("Sản lượng sản xuất");

                entity.Property(e => e.TypeofWine)
                    .HasMaxLength(250)
                    .HasComment("Chủng loại rượu");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.YearReport).HasComment("Năm báo cáo");
            });

            modelBuilder.Entity<CateReportSoldAncol>(entity =>
            {
                entity.ToTable("CateReportSoldAncol");

                entity.Property(e => e.CateReportSoldAncolId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BusinessId).HasComment("Id doanh nghiệp");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.QuantityBoughtOfYear).HasComment("Số lượng mua trong năm");

                entity.Property(e => e.QuantitySoldOfYear).HasComment("Số lượng bán trong năm");

                entity.Property(e => e.TotalPriceBoughtOfYear).HasComment("Tổng giá trị mua trong năm");

                entity.Property(e => e.TotalPriceSoldOfYear).HasComment("Tổng số lượng bán trong năm");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Year).HasComment("Năm báo cáo");
            });

            modelBuilder.Entity<CateReportSoldAncolForFactoryLicense>(entity =>
            {
                entity.ToTable("CateReportSoldAncolForFactoryLicense");

                entity.Property(e => e.CateReportSoldAncolForFactoryLicenseId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BusinessId).HasComment("Id tổ chức / cá nhân");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Quantity).HasComment("Sản lượng");

                entity.Property(e => e.TypeofWine)
                    .HasMaxLength(250)
                    .HasComment("Loại rượu");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.WineFactory)
                    .HasMaxLength(250)
                    .HasComment("Nhà máy mua rượu để chế biến lại");
            });

            modelBuilder.Entity<CateReportTurnOverIndustAncol>(entity =>
            {
                entity.ToTable("CateReportTurnOverIndustAncol");

                entity.Property(e => e.CateReportTurnOverIndustAncolId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BusinessId).HasComment("Id doanh nghiệp / thương nhân");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CateRetail>(entity =>
            {
                entity.ToTable("CateRetail");

                entity.Property(e => e.CateRetailId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng danh mục bán lẻ");

                entity.Property(e => e.CateRetailCode).HasMaxLength(100);

                entity.Property(e => e.ConfirmTime).HasColumnType("datetime");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Importtime)
                    .HasColumnType("datetime")
                    .HasColumnName("importtime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ReportMonth).HasMaxLength(100);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CateRetailDetail>(entity =>
            {
                entity.ToTable("CateRetail_Detail");

                entity.Property(e => e.CateRetailDetailId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CumulativeToReportingMonth).HasComment("Cộng dồn từ đầu năm đến tháng báo cáo");

                entity.Property(e => e.EstimateReportingMonth).HasComment("Ước tính báo cáo tháng");

                entity.Property(e => e.PerformLastmonth).HasComment("Thực hiện tháng trước");

                entity.Property(e => e.PerformReporting).HasComment("Thực hiện báo cáo");

                entity.Property(e => e.Type).HasComment("1: Tổng mức bán lẻ hàng hóa theo năm báo cáo\r\n2: Tổng mức bán lẻ hàng hóa theo năm trước");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.Property(e => e.CategoryId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CategoryCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CategoryName).HasMaxLength(500);

                entity.Property(e => e.CategoryTypeCode)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Piority).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("datetime")
                    .HasComment("Thời gian chỉnh sửa");

                entity.Property(e => e.UpdateUserId).HasComment("Người chỉnh sửa");
            });

            modelBuilder.Entity<CategoryType>(entity =>
            {
                entity.ToTable("CategoryType");

                entity.Property(e => e.CategoryTypeId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CategoryTypeCode)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("Mã nhóm");

                entity.Property(e => e.CategoryTypeName)
                    .HasMaxLength(500)
                    .HasComment("Tên nhóm");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .HasComment("Mô tả");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.UpdateTime)
                    .HasColumnType("datetime")
                    .HasComment("Thời gian chỉnh sửa");

                entity.Property(e => e.UpdateUserId).HasComment("Người chỉnh sửa");
            });

            modelBuilder.Entity<ChemicalBusinessManagement>(entity =>
            {
                entity.ToTable("ChemicalBusinessManagement");

                entity.Property(e => e.ChemicalBusinessManagementId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Quản lý doanh nghiệp hoá chất");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ");

                entity.Property(e => e.BusinessName).HasComment("Tên doanh nghiệp");

                entity.Property(e => e.ChemicalStorage)
                    .HasMaxLength(500)
                    .HasComment("Tồn trữ hoá chất");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Pnupschcmeasures)
                    .HasColumnName("PNUPSCHCMeasures")
                    .HasComment("Xây dựng biện pháp PNUPSCHC ");

                entity.Property(e => e.Represent).HasMaxLength(100);

                entity.Property(e => e.Status).HasComment("Trạng thái");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ChemicalSafetyCertificate>(entity =>
            {
                entity.ToTable("ChemicalSafetyCertificate");

                entity.Property(e => e.ChemicalSafetyCertificateId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng giấy chứng nhận - An toàn hóa chất");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ trụ sở chính");

                entity.Property(e => e.BusinessAddress)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ kinh doanh hóa chất");

                entity.Property(e => e.BusinessCertificateDate)
                    .HasColumnType("date")
                    .HasComment("Ngày cấp giấy chứng nhận đăng ký doanh nghiệp");

                entity.Property(e => e.BusinessCode)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("Mã doanh nghiệp");

                entity.Property(e => e.BusinessId).HasComment("Id doanh nghiệp");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Fax)
                    .HasMaxLength(500)
                    .HasComment("Fax");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.LicenseDate)
                    .HasColumnType("date")
                    .HasComment("Ngày cấp");

                entity.Property(e => e.Num)
                    .HasMaxLength(500)
                    .HasComment("Số cấp");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(11)
                    .HasComment("Số điện thoại");

                entity.Property(e => e.Provider)
                    .HasMaxLength(500)
                    .HasComment("Người cấp");

                entity.Property(e => e.Status)
                    .HasDefaultValueSql("((0))")
                    .HasComment("Trạng thái: 0 - Chưa đủ điều kiện; 1 - Đủ điều kiện (Chưa cấp giấy chứng nhận); 2 - Đã cấp giấy chứng nhận");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.ValidTill)
                    .HasColumnType("date")
                    .HasComment("Hiệu lực đến");
            });

            modelBuilder.Entity<ChemicalSafetyCertificateAttachFile>(entity =>
            {
                entity.ToTable("ChemicalSafetyCertificate_AttachFile");

                entity.Property(e => e.ChemicalSafetyCertificateAttachFileId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LinkFile).HasMaxLength(250);
            });

            modelBuilder.Entity<ChemicalSafetyCertificateChemicalInfo>(entity =>
            {
                entity.HasKey(e => e.ChemicalId);

                entity.ToTable("ChemicalSafetyCertificate_ChemicalInfo");

                entity.Property(e => e.ChemicalId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng thôn tin hóa chất - giấy chứng nhận - An toàn hóa chất");

                entity.Property(e => e.Cascode)
                    .HasMaxLength(100)
                    .HasColumnName("CASCode")
                    .HasComment("Mã CAS");

                entity.Property(e => e.ChemicalFormula)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("Công thức hóa học");

                entity.Property(e => e.ChemicalSafetyCertificateId).HasComment("Id giấy chứng nhận");

                entity.Property(e => e.Content)
                    .HasMaxLength(100)
                    .HasComment("Hàm lượng");

                entity.Property(e => e.Mass)
                    .HasMaxLength(100)
                    .HasComment("Khối lượng (Tấn/Năm)");

                entity.Property(e => e.NameOfChemical)
                    .HasMaxLength(500)
                    .HasComment("Tên hóa chất");

                entity.Property(e => e.TradeName)
                    .HasMaxLength(500)
                    .HasComment("Tên thương mại");
            });

            modelBuilder.Entity<CigaretteBusiness>(entity =>
            {
                entity.ToTable("Cigarette_Business");

                entity.Property(e => e.CigaretteBusinessId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.PhoneNumber).HasMaxLength(11);

                entity.Property(e => e.Representative).HasMaxLength(200);

                entity.Property(e => e.Supplier).HasMaxLength(200);

                entity.Property(e => e.SupplierIdAddress).HasMaxLength(200);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CigaretteBusinessStore>(entity =>
            {
                entity.HasKey(e => e.CigaretteDetailId);

                entity.ToTable("Cigarette_Business_Store");

                entity.Property(e => e.CigaretteDetailId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DiaChi).HasMaxLength(200);

                entity.Property(e => e.DiaChiDonViCungCap).HasMaxLength(100);

                entity.Property(e => e.DonViCungCap).HasMaxLength(200);

                entity.Property(e => e.GhiChu).HasMaxLength(100);

                entity.Property(e => e.GiayPhepKinhDoanh).HasMaxLength(50);

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.NgayCap).HasColumnType("datetime");

                entity.Property(e => e.NgayHetHan).HasColumnType("datetime");

                entity.Property(e => e.NguoiDaiDien).HasMaxLength(200);

                entity.Property(e => e.PhoneDonViCungCap).HasMaxLength(100);

                entity.Property(e => e.SoDienThoai).HasMaxLength(20);

                entity.Property(e => e.TenDoanhNghiep).HasMaxLength(200);
            });

            modelBuilder.Entity<CommercialManagement>(entity =>
            {
                entity.HasKey(e => e.CommercialId);

                entity.ToTable("CommercialManagement");

                entity.Property(e => e.CommercialId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Code).HasMaxLength(500);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Fax).HasMaxLength(500);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.PhoneNumber).HasMaxLength(11);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CommitManager>(entity =>
            {
                entity.ToTable("CommitManager");

                entity.Property(e => e.CommitManagerId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CoSo).HasMaxLength(100);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DiaChi).HasMaxLength(200);

                entity.Property(e => e.GhiChu).HasMaxLength(500);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.MaHoSo).HasMaxLength(50);

                entity.Property(e => e.NgayCamKet).HasColumnType("datetime");

                entity.Property(e => e.NgayNhanHoSo).HasColumnType("datetime");

                entity.Property(e => e.NguoiLamCamKet).HasMaxLength(50);

                entity.Property(e => e.SoDienThoai).HasMaxLength(20);

                entity.Property(e => e.TenThuTuc).HasMaxLength(100);

                entity.Property(e => e.TenToChuc).HasMaxLength(100);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CommitManagerListItem>(entity =>
            {
                entity.ToTable("CommitManagerListItem");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.TenMatHang).HasMaxLength(100);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Commune>(entity =>
            {
                entity.ToTable("Commune");

                entity.Property(e => e.CommuneId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CommuneCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CommuneName).HasMaxLength(500);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<CommuneElectricityManagement>(entity =>
            {
                entity.ToTable("CommuneElectricityManagement");

                entity.Property(e => e.CommuneElectricityManagementId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng quản lý điện cấp xã");

                entity.Property(e => e.CommuneId).HasComment("Mã xã lấy từ bảng Commune");

                entity.Property(e => e.Content41End).HasComment("Đạt nội dung 4.1");

                entity.Property(e => e.Content41Start).HasComment("Đạt nội dung 4.1");

                entity.Property(e => e.Content42End).HasComment("Đạt nội dung 4.2");

                entity.Property(e => e.Content42Start).HasComment("Đạt nội dung 4.2");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DistrictId).HasComment("ID Huyện lấy từ bảng District");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note)
                    .HasMaxLength(500)
                    .HasComment("Ghi chú");

                entity.Property(e => e.StageId).HasComment("ID giai đoạn lấy từ bảng Stage");

                entity.Property(e => e.Target4End).HasComment("Đạt tiêu chí số 4");

                entity.Property(e => e.Target4Start).HasComment("Đạt tiêu chí số 4");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ConsumerServiceRevenue>(entity =>
            {
                entity.ToTable("ConsumerServiceRevenue");

                entity.Property(e => e.ConsumerServiceRevenueId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng danh mục bán lẻ");

                entity.Property(e => e.ConfirmTime).HasColumnType("datetime");

                entity.Property(e => e.ConsumerServiceRevenueCode).HasMaxLength(100);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Importtime)
                    .HasColumnType("datetime")
                    .HasColumnName("importtime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ReportMonth).HasMaxLength(100);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ConsumerServiceRevenueDetail>(entity =>
            {
                entity.ToTable("ConsumerServiceRevenue_Detail");

                entity.Property(e => e.ConsumerServiceRevenueDetailId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CumulativeToReportingMonth).HasComment("Cộng dồn từ đầu năm đến tháng báo cáo");

                entity.Property(e => e.EstimateReportingMonth).HasComment("Ước tính báo cáo tháng");

                entity.Property(e => e.PerformLastmonth).HasComment("Thực hiện tháng trước");

                entity.Property(e => e.PerformReporting).HasComment("Thực hiện báo cáo");

                entity.Property(e => e.Type).HasComment("1: Tổng mức bán lẻ hàng hóa theo năm báo cáo\r\n2: Tổng mức bán lẻ hàng hóa theo năm trước");
            });

            modelBuilder.Entity<Coordinate>(entity =>
            {
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address)
                    .HasMaxLength(250)
                    .HasComment("Địa chỉ của Item");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.FormattedAddress)
                    .HasMaxLength(250)
                    .HasComment("Địa chỉ của Geocode");

                entity.Property(e => e.Icon)
                    .HasMaxLength(50)
                    .HasComment("Icon hiển thị");

                entity.Property(e => e.ItemId).HasComment("Id của Item cần lưu tọa độ");

                entity.Property(e => e.Lat)
                    .HasColumnType("decimal(18, 15)")
                    .HasComment("Vĩ độ của Item");

                entity.Property(e => e.Lng)
                    .HasColumnType("decimal(18, 15)")
                    .HasComment("Tung độ của Item");

                entity.Property(e => e.TableCode)
                    .HasMaxLength(50)
                    .HasComment("Bảng lưu Item");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country");

                entity.Property(e => e.CountryId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("@Mã quốc gia@");

                entity.Property(e => e.CountryName)
                    .HasMaxLength(500)
                    .HasComment("@Tên quốc gia@");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Departmemt>(entity =>
            {
                entity.ToTable("Departmemt");

                entity.Property(e => e.DepartmemtId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DepartmemtCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DepartmemtName).HasMaxLength(500);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.ToTable("District");

                entity.Property(e => e.DistrictId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DistrictCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DistrictName).HasMaxLength(500);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<DistrictCustom>(entity =>
            {
                entity.ToTable("DistrictCustom");

                entity.Property(e => e.DistrictCustomId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DistrictCustomCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DistrictCustomName).HasMaxLength(500);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ElectricOperatingUnit>(entity =>
            {
                entity.HasKey(e => e.ElectricOperatingUnitsId);

                entity.Property(e => e.ElectricOperatingUnitsId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng các đơn vị hoạt động điện lực");

                entity.Property(e => e.Address).HasComment("Địa chỉ");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.CustomerName).HasComment("Id doanh nghiệp");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsConsulting).HasComment("Tư vấn thiết kế");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.IsElectricityRetail).HasComment("Bán lẻ điện");

                entity.Property(e => e.IsPowerDistribution).HasComment("Phân phối điện");

                entity.Property(e => e.IsPowerGeneration).HasComment("Phát điện");

                entity.Property(e => e.IsSurveillance).HasComment("Tư vấn giám sát");

                entity.Property(e => e.NumOfGp)
                    .HasMaxLength(500)
                    .HasColumnName("NumOfGP")
                    .HasComment("Số của GP hoạt động điện lực");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasComment("Số điện thoại");

                entity.Property(e => e.PresidentName)
                    .HasMaxLength(500)
                    .HasComment("Tên giám đốc");

                entity.Property(e => e.SignDay)
                    .HasColumnType("date")
                    .HasComment("Ngày ký văn bản");

                entity.Property(e => e.Status).HasComment("Tình trạng hoạt động: 0 - Còn hoạt động; 1 - Ngừng hoạt động");

                entity.Property(e => e.Supplier).HasComment("Đơn vị cấp");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ElectricalProjectManagement>(entity =>
            {
                entity.ToTable("ElectricalProjectManagement");

                entity.Property(e => e.ElectricalProjectManagementId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng quản lý công trình điện 110 KV và 220 KV trên tỉnh");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Địa điểm");

                entity.Property(e => e.BuildingCode)
                    .HasMaxLength(500)
                    .HasComment("Mã công trình");

                entity.Property(e => e.BuildingName)
                    .HasMaxLength(500)
                    .HasComment("Tên công trình");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.District).HasComment("Huyện");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Length).HasMaxLength(20);

                entity.Property(e => e.Note)
                    .HasMaxLength(500)
                    .HasComment("Ghi chú");

                entity.Property(e => e.Represent)
                    .HasMaxLength(500)
                    .HasComment("Người đại diện");

                entity.Property(e => e.Status).HasComment("Trạng thái: 1: Hoạt động, 2: Tạm ngừng, 3 Ngừng hoạt động");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Wattage).HasMaxLength(20);

                entity.Property(e => e.WireType).HasMaxLength(20);
            });

            modelBuilder.Entity<ElectricityInspectorCard>(entity =>
            {
                entity.ToTable("ElectricityInspectorCard");

                entity.Property(e => e.ElectricityInspectorCardId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Danh sách thẻ kiểm tra viên điện lực");

                entity.Property(e => e.Birthday)
                    .HasColumnType("date")
                    .HasComment("Ngày sinh");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Degree)
                    .HasMaxLength(500)
                    .HasComment("Trình độ");

                entity.Property(e => e.InspectorName)
                    .HasMaxLength(500)
                    .HasComment("Tên Kiểm tra viên");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.LicenseDate)
                    .HasColumnType("date")
                    .HasComment("Ngày cấp thẻ");

                entity.Property(e => e.Seniority)
                    .HasMaxLength(500)
                    .HasComment("Thâm niên");

                entity.Property(e => e.Unit)
                    .HasMaxLength(500)
                    .HasComment("Đơn vị");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<EnergyIndustry>(entity =>
            {
                entity.ToTable("EnergyIndustry");

                entity.Property(e => e.EnergyIndustryId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.EnergyIndustryCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.EnergyIndustryName).HasMaxLength(500);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<EnvironmentProjectManagement>(entity =>
            {
                entity.ToTable("EnvironmentProjectManagement");

                entity.Property(e => e.EnvironmentProjectManagementId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Quản lý đề án");

                entity.Property(e => e.ApprovedFunding)
                    .HasMaxLength(500)
                    .HasComment("Kinh phí được duyệt");

                entity.Property(e => e.CoordinationUnit)
                    .HasMaxLength(500)
                    .HasComment("Đơn vị phối hợp");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.ImplementationContent)
                    .HasMaxLength(500)
                    .HasComment("Nội dung thực hiện");

                entity.Property(e => e.ImplementationCost)
                    .HasMaxLength(500)
                    .HasComment("Kinh phí thực hiện");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ProjectName)
                    .HasMaxLength(250)
                    .HasComment("Tên dự án / đề án");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.YearOfImplementationFrom).HasComment("Năm thực hiện");
            });

            modelBuilder.Entity<EnvironmentProjectManagementAttachFile>(entity =>
            {
                entity.ToTable("EnvironmentProjectManagement_AttachFile");

                entity.Property(e => e.EnvironmentProjectManagementAttachFileId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LinkFile).HasMaxLength(250);
            });

            modelBuilder.Entity<ExportGood>(entity =>
            {
                entity.HasKey(e => e.ExportGoodsId);

                entity.Property(e => e.ExportGoodsId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.BusinessId).HasComment("Get data from Table Business");

                entity.Property(e => e.CountryId).HasComment("Get data from Table Country");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.ExportGoodsName).HasMaxLength(500);

                entity.Property(e => e.ExportTime).HasColumnType("date");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ItemGroupId).HasComment("Get data from Table Category");

                entity.Property(e => e.Price).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.TypeOfEconomicId).HasComment("Get data from Table Category");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<FinancialPlanTarget>(entity =>
            {
                entity.HasKey(e => e.FinancialPlanTargetsId);

                entity.Property(e => e.FinancialPlanTargetsId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng các chỉ tiêu sản xuất kinh doanh, xuất khẩu chủ yếu");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.CumulativeToMonth)
                    .HasColumnType("decimal(18, 0)")
                    .HasComment("Cộng dồn đến tháng");

                entity.Property(e => e.EstimatedMonth)
                    .HasColumnType("decimal(18, 0)")
                    .HasComment("Ước tính tháng thực hiện");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Name)
                    .HasMaxLength(500)
                    .HasComment("Tên");

                entity.Property(e => e.Planning).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Type).HasComment("Loại hình: 1 - Giá trị sản xuất, 2 - Sản phẩm chủ yếu, 3 - Khối Doanh nghiệp (Xuất khẩu), 4 - Nhóm hàng (Xuất khẩu), 5 - Mặt hàng (Xuất khẩu), 6 - Thị trường (Xuất khẩu), 7 - Mặt hàng  chủ yếu (Nhập khẩu)");

                entity.Property(e => e.Unit)
                    .HasMaxLength(100)
                    .HasComment("Đơn vị tính");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Year).HasComment("Năm báo cáo");
            });

            modelBuilder.Entity<FoodSafetyCertificate>(entity =>
            {
                entity.ToTable("FoodSafetyCertificate");

                entity.Property(e => e.FoodSafetyCertificateId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng giấy chứng nhận - An toàn thực phẩm");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ sản xuất");

                entity.Property(e => e.BusinessId).HasComment("Id doanh nghiệp");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.LicenseDate).HasColumnType("date");

                entity.Property(e => e.ManagerName)
                    .HasMaxLength(500)
                    .HasComment("Tên chủ sở hữu");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.Num)
                    .HasMaxLength(500)
                    .HasComment("Số cấp");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(11)
                    .HasComment("Số điện thoại");

                entity.Property(e => e.ProfileCode).HasMaxLength(50);

                entity.Property(e => e.ProfileName).HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasDefaultValueSql("((0))")
                    .HasComment("Trạng thái: 0 - Chưa đủ điều kiện; 1 - Đủ điều kiện (Chưa cấp giấy chứng nhận); 2 - Đã cấp giấy chứng nhận");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.ValidTill)
                    .HasColumnType("date")
                    .HasComment("Hiệu lực đến");
            });

            modelBuilder.Entity<FoodSafetyCertificateAttachFile>(entity =>
            {
                entity.ToTable("FoodSafetyCertificateAttachFile");

                entity.Property(e => e.FoodSafetyCertificateAttachFileId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LinkFile).HasMaxLength(250);
            });

            modelBuilder.Entity<FoodSafetyCertificateItem>(entity =>
            {
                entity.HasKey(e => e.ItemId);

                entity.Property(e => e.ItemId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Id Item");

                entity.Property(e => e.FoodSafetyCertificateId).HasComment("Id giấy chứng nhận");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(250)
                    .HasComment("Tên loại hình");

                entity.Property(e => e.Type).HasComment("Id loại hình");
            });

            modelBuilder.Entity<Ga>(entity =>
            {
                entity.HasKey(e => e.GasId);

                entity.Property(e => e.GasId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<GasBusiness>(entity =>
            {
                entity.ToTable("GasBusiness");

                entity.Property(e => e.GasBusinessId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DateEnd).HasColumnType("datetime");

                entity.Property(e => e.DateStart).HasColumnType("datetime");

                entity.Property(e => e.Fax).HasMaxLength(20);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Licensors).HasMaxLength(100);

                entity.Property(e => e.NumDoc).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<GasTrainingClassManagement>(entity =>
            {
                entity.ToTable("GasTrainingClassManagement");

                entity.Property(e => e.GasTrainingClassManagementId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Location).HasMaxLength(500);

                entity.Property(e => e.Participant).HasMaxLength(500);

                entity.Property(e => e.TimeStart).HasColumnType("datetime");

                entity.Property(e => e.Topic).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<GasTrainingClassManagementAttachFile>(entity =>
            {
                entity.ToTable("GasTrainingClassManagement_AttachFile");

                entity.Property(e => e.GasTrainingClassManagementAttachFileId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LinkFile).HasMaxLength(250);
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.ToTable("Group");

                entity.Property(e => e.GroupId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.GroupName).HasMaxLength(500);

                entity.Property(e => e.Priority).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<GroupPermit>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.Code });

                entity.ToTable("GroupPermit");

                entity.Property(e => e.Code).HasMaxLength(50);
            });

            modelBuilder.Entity<ImportDonvi>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("import_donvi");

                entity.Property(e => e.Id)
                    .HasMaxLength(500)
                    .HasColumnName("id");

                entity.Property(e => e.Text)
                    .HasMaxLength(500)
                    .HasColumnName("text");
            });

            modelBuilder.Entity<ImportGood>(entity =>
            {
                entity.HasKey(e => e.ImportGoodsId);

                entity.Property(e => e.ImportGoodsId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Amount).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.BusinessId).HasComment("Get data from Table Business");

                entity.Property(e => e.CountryId).HasComment("Get data from Table Country");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.ImportGoodsName).HasMaxLength(500);

                entity.Property(e => e.ImportTime).HasColumnType("date");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ItemGroupId).HasComment("Get data from Table Category");

                entity.Property(e => e.Price).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.TypeOfEconomicId).HasComment("Get data from Table Category");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ImportLoaihinhXdnc>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("import_loaihinh_xdnc");

                entity.Property(e => e.Id)
                    .HasMaxLength(500)
                    .HasColumnName("id");

                entity.Property(e => e.Text)
                    .HasMaxLength(500)
                    .HasColumnName("TEXT");
            });

            modelBuilder.Entity<ImportTemp>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("import_temp");

                entity.Property(e => e.Id)
                    .HasMaxLength(500)
                    .HasColumnName("id");

                entity.Property(e => e.Text)
                    .HasMaxLength(500)
                    .HasColumnName("text");
            });

            modelBuilder.Entity<IndustrialManagementTarget>(entity =>
            {
                entity.ToTable("IndustrialManagementTarget");

                entity.Property(e => e.IndustrialManagementTargetId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Unit).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<IndustrialManagementTargetChild>(entity =>
            {
                entity.ToTable("IndustrialManagementTargetChild");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Unit).HasMaxLength(200);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<IndustrialPromotionFundingReport>(entity =>
            {
                entity.HasKey(e => e.RpIndustrialPromotionFundingId);

                entity.ToTable("IndustrialPromotionFundingReport");

                entity.Property(e => e.RpIndustrialPromotionFundingId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.LocalReport).HasComment("Số lượng khuyến công địa phương");

                entity.Property(e => e.NationalReport).HasComment("Số lượng khuyến công quốc gia");

                entity.Property(e => e.Targets).HasComment("Chỉ tiêu");

                entity.Property(e => e.Unit)
                    .HasMaxLength(50)
                    .HasComment("Đơn vị tính");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.YearReport).HasComment("Năm báo cáo");
            });

            modelBuilder.Entity<IndustrialPromotionProject>(entity =>
            {
                entity.ToTable("IndustrialPromotionProject");

                entity.Property(e => e.IndustrialPromotionProjectId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Capital).HasComment("1: Trung ương\r\n2: Địa phương");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.EndDate).HasColumnType("datetime");

                entity.Property(e => e.Funding).HasComment("Tổng kinh phí");

                entity.Property(e => e.IndustrialPromotionFunding).HasComment("Kinh phí khuyến công hỗ trợ");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ProjectName).HasMaxLength(250);

                entity.Property(e => e.ReciprocalEnterpriseFunding).HasComment("Kinh phí doanh nghiệp đối ứng");

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<IndustrialPromotionProjectDetail>(entity =>
            {
                entity.ToTable("IndustrialPromotionProjectDetail");

                entity.Property(e => e.IndustrialPromotionProjectDetailId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BusinessCode).HasMaxLength(250);

                entity.Property(e => e.BusinessNameVi).HasMaxLength(250);

                entity.Property(e => e.DiaChi).HasMaxLength(250);

                entity.Property(e => e.NganhNghe).HasMaxLength(250);

                entity.Property(e => e.NguoiDaiDien).HasMaxLength(250);
            });

            modelBuilder.Entity<IndustrialPromotionResultsReport>(entity =>
            {
                entity.HasKey(e => e.RpIndustrialPromotionResultsId);

                entity.ToTable("IndustrialPromotionResultsReport");

                entity.Property(e => e.RpIndustrialPromotionResultsId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.LocalReport).HasComment("Số lượng khuyến công địa phương");

                entity.Property(e => e.NationalReport).HasComment("Số lượng khuyến công quốc gia");

                entity.Property(e => e.Targets).HasComment("Chỉ tiêu");

                entity.Property(e => e.Unit)
                    .HasMaxLength(50)
                    .HasComment("Đơn vị tính");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.YearReport).HasComment("Năm báo cáo");
            });

            modelBuilder.Entity<Industry>(entity =>
            {
                entity.ToTable("Industry");

                entity.Property(e => e.IndustryId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IndustryCode)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("@Mã ngành Nghề@ $exist$");

                entity.Property(e => e.IndustryName)
                    .HasMaxLength(500)
                    .HasComment("@Tên ngành Nghề@");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ParentIndustryId).HasComment("@Ngành nghề cha *@ #IndustryName#");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<InternationalCommerce>(entity =>
            {
                entity.ToTable("International_Commerce");

                entity.Property(e => e.InternationalCommerceId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DiaChiCoSoBanLe).HasMaxLength(500);

                entity.Property(e => e.DienTichBanHang).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.DienTichKinhDoanh).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.DienTichSan).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.DienTichSuDung).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.GhiChu).HasMaxLength(500);

                entity.Property(e => e.GiayPhepBanLe).HasMaxLength(100);

                entity.Property(e => e.GiayPhepKinhDoanh).HasMaxLength(100);

                entity.Property(e => e.InvestorName).HasMaxLength(200);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.LicensingActivity).HasMaxLength(200);

                entity.Property(e => e.NgayCapGiayPhepBanLe).HasColumnType("datetime");

                entity.Property(e => e.NgayCapGiayPhepKinhDoanh).HasColumnType("datetime");

                entity.Property(e => e.NgayHetHanGiayPhepBanLe).HasColumnType("datetime");

                entity.Property(e => e.TenCoSoBanLe).HasMaxLength(100);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ListOfKeyEnergyUser>(entity =>
            {
                entity.HasKey(e => e.ListOfKeyEnergyUsersId);

                entity.Property(e => e.ListOfKeyEnergyUsersId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng quản lý cơ sở sử dụng năng lượng trọng điểm");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ");

                entity.Property(e => e.BusinessId).HasComment("Id doanh nghiệp");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Decision)
                    .HasMaxLength(500)
                    .HasComment("Quyết định CSNLTĐ");

                entity.Property(e => e.EnergyConsumption).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Link)
                    .HasMaxLength(500)
                    .HasComment("Link doanh nghiệp");

                entity.Property(e => e.ManufactProfession).HasMaxLength(100);

                entity.Property(e => e.Note).HasMaxLength(200);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<LoginSession>(entity =>
            {
                entity.ToTable("LoginSession");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(dateadd(day,(7),getdate()))");

                entity.Property(e => e.LoginDate).HasColumnType("datetime");

                entity.Property(e => e.Token).HasMaxLength(500);
            });

            modelBuilder.Entity<MainMenu>(entity =>
            {
                entity.HasKey(e => e.IdMain);

                entity.ToTable("MainMenu");

                entity.Property(e => e.AllowPermit).HasMaxLength(50);

                entity.Property(e => e.GroupName).HasMaxLength(500);

                entity.Property(e => e.Icon).HasMaxLength(500);

                entity.Property(e => e.Link).HasMaxLength(500);

                entity.Property(e => e.Summary).HasMaxLength(500);

                entity.Property(e => e.Target).HasMaxLength(500);

                entity.Property(e => e.Title).HasMaxLength(500);
            });

            modelBuilder.Entity<ManageArchiveRecord>(entity =>
            {
                entity.HasKey(e => e.ManageArchiveRecordsId);

                entity.Property(e => e.ManageArchiveRecordsId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng quản lý lưu trữ hồ sơ");

                entity.Property(e => e.CodeFile)
                    .HasMaxLength(100)
                    .HasComment("Số và ký hiệu hồ sơ");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Creator).HasComment("Đơn vị, người lập hồ sơ");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Location)
                    .HasMaxLength(500)
                    .HasComment("Địa điểm");

                entity.Property(e => e.Note)
                    .HasMaxLength(500)
                    .HasComment("Ghi chú");

                entity.Property(e => e.ReceptionTime)
                    .HasColumnType("datetime")
                    .HasComment("Thơi gian tiếp nhận");

                entity.Property(e => e.RecordsFinancePlanId).HasComment("Mã nhóm hồ sơ");

                entity.Property(e => e.StorageTime).HasComment("Thời gian bảo quản");

                entity.Property(e => e.StoreDocumentsAt).HasMaxLength(500);

                entity.Property(e => e.StoreFilesAt).HasMaxLength(500);

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .HasComment("Tiêu đề hồ sơ");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ManageConfirmPromotion>(entity =>
            {
                entity.ToTable("ManageConfirmPromotion");

                entity.Property(e => e.ManageConfirmPromotionId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.GoodsServices).HasMaxLength(500);

                entity.Property(e => e.GoodsServicesPay).HasMaxLength(500);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.ManageConfirmPromotionName).HasMaxLength(100);

                entity.Property(e => e.NumberOfDocuments)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TimeEnd).HasColumnType("datetime");

                entity.Property(e => e.TimeStart).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ManageConfirmPromotionAttachFile>(entity =>
            {
                entity.ToTable("ManageConfirmPromotion_AttachFile");

                entity.Property(e => e.ManageConfirmPromotionAttachFileId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LinkFile).HasMaxLength(250);
            });

            modelBuilder.Entity<ManagementElectricityActivity>(entity =>
            {
                entity.HasKey(e => e.ManagementElectricityActivitiesId);

                entity.Property(e => e.ManagementElectricityActivitiesId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DateOfAcceptance).HasColumnType("datetime");

                entity.Property(e => e.DistrictId).HasComment("@Tên quốc gia@");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ProjectName)
                    .HasMaxLength(500)
                    .HasComment("@Mã quốc gia@");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ManagementFirePrevention>(entity =>
            {
                entity.ToTable("ManagementFirePrevention");

                entity.Property(e => e.ManagementFirePreventionId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Quản lý công tác phòng chống cháy nổ thuộc ngành công thương");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ");

                entity.Property(e => e.BusinessName)
                    .HasMaxLength(500)
                    .HasComment("Tên doanh nghiệp");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Reality).HasComment("Thực trạng: 2 - Tốt, 1 - Trung Bình, 0 - Không đạt");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ManagementSeminar>(entity =>
            {
                entity.ToTable("ManagementSeminar");

                entity.Property(e => e.ManagementSeminarId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.Contact).HasMaxLength(50);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note).HasMaxLength(200);

                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.Property(e => e.ProfileCode).HasMaxLength(100);

                entity.Property(e => e.Title).HasMaxLength(200);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<MarketDevelopPlan>(entity =>
            {
                entity.ToTable("MarketDevelopPlan");

                entity.Property(e => e.MarketDevelopPlanId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AddLandArea).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.Capital).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.ExistLandArea).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.MarketName).HasMaxLength(200);

                entity.Property(e => e.NewLandArea).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<MarketInvestEnterprise>(entity =>
            {
                entity.ToTable("MarketInvestEnterprise");

                entity.Property(e => e.MarketInvestEnterpriseId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.BusinessName).HasMaxLength(200);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Evaluate).HasMaxLength(100);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.MarketName).HasMaxLength(200);

                entity.Property(e => e.Note).HasMaxLength(100);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<MarketManagement>(entity =>
            {
                entity.ToTable("MarketManagement");

                entity.Property(e => e.MarketManagementId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BoothNumber).HasDefaultValueSql("((0))");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DeXuatGiaMoi).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.GiaNgoaiNhaLong).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.GiaTrongNhaLong).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<MarketManagementDetail>(entity =>
            {
                entity.ToTable("MarketManagementDetail");

                entity.Property(e => e.MarketManagementDetailId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BusinessLineName).HasMaxLength(100);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            });

            modelBuilder.Entity<MarketPlanInformation>(entity =>
            {
                entity.ToTable("MarketPlanInformation");

                entity.Property(e => e.MarketPlanInformationId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.BusinessLandArea).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.LandArea).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.MarketName).HasMaxLength(200);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<MarketTargetSeven>(entity =>
            {
                entity.ToTable("MarketTargetSeven");

                entity.Property(e => e.MarketTargetSevenId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.MarketName).HasMaxLength(200);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Year).HasComment("Năm báo cáo");
            });

            modelBuilder.Entity<MeaMonthReport>(entity =>
            {
                entity.HasKey(e => e.ReportMonthId);

                entity.ToTable("MEA_MonthReport");

                entity.Property(e => e.ReportMonthId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Id Tháng báo cáo");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Month).HasComment("Tháng");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("date")
                    .HasComment("Ngày cập nhật văn bản");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Year).HasComment("Năm");
            });

            modelBuilder.Entity<MeaMonthReportAttachFile>(entity =>
            {
                entity.HasKey(e => e.FileId);

                entity.ToTable("MEA_MonthReport_AttachFile");

                entity.Property(e => e.FileId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Id File Văn bản liên quan");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.LinkFile)
                    .HasMaxLength(250)
                    .HasComment("Url của File");

                entity.Property(e => e.MonthReportId).HasComment("Id Tháng báo cáo");
            });

            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasKey(e => e.IdModule);

                entity.ToTable("Module");

                entity.Property(e => e.AllowPermit).HasMaxLength(500);

                entity.Property(e => e.Icon).HasMaxLength(500);

                entity.Property(e => e.Link).HasMaxLength(500);

                entity.Property(e => e.Summary).HasMaxLength(500);

                entity.Property(e => e.Target).HasMaxLength(500);

                entity.Property(e => e.Title).HasMaxLength(500);
            });

            modelBuilder.Entity<MultiLevelSalesManagement>(entity =>
            {
                entity.ToTable("MultiLevelSalesManagement");

                entity.Property(e => e.MultiLevelSalesManagementId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Quản lý cơ sở hoạt động bán hàng đa cấp");

                entity.Property(e => e.BasicTrainings).HasComment("Số lượng đào tạo căn bản");

                entity.Property(e => e.BusinessId).HasComment("Doanh nghiệp");

                entity.Property(e => e.BuyBackGoods).HasComment("Mua lại hàng hoá từ người tham gia bán hàng đa cấp (Triệu đồng)");

                entity.Property(e => e.Commission).HasComment("Tổng hoa hồng, tiền thưởng, lợi ích kinh tế đã nhận (Triệu đồng)");

                entity.Property(e => e.ContactPersonAddress)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ người liên hệ");

                entity.Property(e => e.ContactPersonName)
                    .HasMaxLength(500)
                    .HasComment("Người liên hệ");

                entity.Property(e => e.ContactPersonPhoneNumber)
                    .HasMaxLength(20)
                    .HasComment("Số điện thoại người liên hệ");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.MultiLevelSellingPlace)
                    .HasMaxLength(500)
                    .HasComment("Địa điểm hoạt động bán hàng đa cấp");

                entity.Property(e => e.NewParticipants).HasComment("Số người tham gia bán hàng đa cấp phát sinh mới");

                entity.Property(e => e.Participants).HasComment("Số người tham gia bán hàng đa cấp");

                entity.Property(e => e.PromotionalValue).HasComment("Giá trị khuyến mãi quy đổi thành tiền (Triệu đồng)");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày bắt đầu hoạt động");

                entity.Property(e => e.TaxDeduction).HasComment("Khấu trừ thuế thu nhập cá nhân (Triệu đồng)");

                entity.Property(e => e.Terminations).HasComment("Số người tham gia bán hàng đa cấp chấm dứt hợp đồng");

                entity.Property(e => e.Turnover).HasComment("Doanh thu bán hàng đa cấp trên địa bàn tỉnh (triệu đồng)");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.YearReport)
                    .HasDefaultValueSql("(datepart(year,getdate()))")
                    .HasComment("Năm báo cáo");
            });

            modelBuilder.Entity<MultiLevelSalesParticipant>(entity =>
            {
                entity.HasKey(e => e.MultiLevelSalesParticipantsId);

                entity.Property(e => e.MultiLevelSalesParticipantsId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Quản lý người tham gia bán hàng đa cấp");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ cư trú");

                entity.Property(e => e.Birthday)
                    .HasColumnType("datetime")
                    .HasComment("Ngày sinh");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DateOfIssuance)
                    .HasColumnType("datetime")
                    .HasComment("Ngày cấp CMND / CCCD");

                entity.Property(e => e.Gender).HasComment("Giới tính: 1 - Nam, 2 - Nữ.");

                entity.Property(e => e.IdentityCardNumber)
                    .HasMaxLength(20)
                    .HasComment("Số CMND / CCCD");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.JoinDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày tham gia");

                entity.Property(e => e.MultiLevelSalesParticipantsCode)
                    .HasMaxLength(100)
                    .HasComment("Mã số");

                entity.Property(e => e.ParticipantsName)
                    .HasMaxLength(500)
                    .HasComment("Tên đơn vị");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .HasComment("Số điện thoại");

                entity.Property(e => e.PlaceOfIssue)
                    .HasMaxLength(500)
                    .HasComment("Nơi cấp CMND / CCCD");

                entity.Property(e => e.Province).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<NewRuralCriterion>(entity =>
            {
                entity.HasKey(e => e.NewRuralCriteriaId);

                entity.Property(e => e.NewRuralCriteriaId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("BảngTiêu chí nông thôn mới nông thôn mới nâng cao");

                entity.Property(e => e.CommuneId).HasComment("Mã xã lấy từ bảng Commune");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DistrictId).HasComment("ID Huyện lấy từ bảng District");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note).HasComment("Ghi chú");

                entity.Property(e => e.Target1708).HasComment("Đạt tiêu chí 1708");

                entity.Property(e => e.Target4).HasComment("Đạt tiêu chí số 4");

                entity.Property(e => e.Target7).HasComment("Đạt tiêu chí số 7");

                entity.Property(e => e.Title).HasComment("Danh hiệu: 1 - Nông thôn mới, 2 - Nông thôn mới nâng cao");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ParticipateSupportFair>(entity =>
            {
                entity.ToTable("ParticipateSupportFair");

                entity.Property(e => e.ParticipateSupportFairId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address)
                    .HasMaxLength(250)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.ImplementCost).HasComment("Kinh phí thực hiện");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ParticipateSupportFairName).HasMaxLength(250);

                entity.Property(e => e.PlanJoin).HasComment("1: Sở tham gia\r\n2: Hỗ trợ doanh nghiệp tham gia");

                entity.Property(e => e.Scale).HasMaxLength(250);

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ParticipateSupportFairDetail>(entity =>
            {
                entity.ToTable("ParticipateSupportFairDetail");

                entity.Property(e => e.ParticipateSupportFairDetailId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BusinessCode).HasMaxLength(250);

                entity.Property(e => e.BusinessNameVi).HasMaxLength(250);

                entity.Property(e => e.DiaChi).HasMaxLength(250);

                entity.Property(e => e.Huyen).HasMaxLength(50);

                entity.Property(e => e.NganhNghe).HasMaxLength(250);

                entity.Property(e => e.NguoiDaiDien).HasMaxLength(250);

                entity.Property(e => e.Xa).HasMaxLength(50);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("Permission");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CodeGroup).HasMaxLength(500);

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Disable).HasDefaultValueSql("((0))");

                entity.Property(e => e.PermitName).HasMaxLength(500);
            });

            modelBuilder.Entity<PetroleumBusiness>(entity =>
            {
                entity.ToTable("Petroleum_Business");

                entity.Property(e => e.PetroleumBusinessId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.JoinZaloOa).HasColumnName("JoinZaloOA");

                entity.Property(e => e.PhoneNumber).HasMaxLength(11);

                entity.Property(e => e.Representative).HasMaxLength(100);

                entity.Property(e => e.Supplier)
                    .HasMaxLength(200)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.TimeRegister).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<PetroleumBusinessStore>(entity =>
            {
                entity.HasKey(e => e.PetroleumDetailId)
                    .HasName("PK_PetroleumBu_Store");

                entity.ToTable("Petroleum_Business_Store");

                entity.Property(e => e.PetroleumDetailId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.DiaChi).HasMaxLength(200);

                entity.Property(e => e.DiaChiDonViCungCap).HasMaxLength(100);

                entity.Property(e => e.DienTichXayDung).HasMaxLength(200);

                entity.Property(e => e.DonViCungCap).HasMaxLength(200);

                entity.Property(e => e.GhiChu).HasMaxLength(200);

                entity.Property(e => e.GiayPhepKinhDoanh).HasMaxLength(50);

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.NgayCapPhep).HasColumnType("datetime");

                entity.Property(e => e.NgayCapPhepXayDung).HasColumnType("datetime");

                entity.Property(e => e.NgayHetHan).HasColumnType("datetime");

                entity.Property(e => e.NguoiDaiDien).HasMaxLength(200);

                entity.Property(e => e.NguoiLienHeDonViCungCap).HasMaxLength(50);

                entity.Property(e => e.NguoiQuanLy).HasMaxLength(200);

                entity.Property(e => e.SoDienThoai).HasMaxLength(20);

                entity.Property(e => e.SoDienThoaiDonViCungCap).HasMaxLength(20);

                entity.Property(e => e.TenCuaHang).HasMaxLength(200);

                entity.Property(e => e.ThoiGianBanHang).HasMaxLength(200);

                entity.Property(e => e.ThoiHan1Nam).HasColumnType("datetime");

                entity.Property(e => e.ThoiHan5Nam).HasColumnType("datetime");

                entity.Property(e => e.TuyenPhucVu).HasMaxLength(200);
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.ToTable("Position");

                entity.Property(e => e.PositionId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.PositionCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.PositionName).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProcessAdministrativeProcedure>(entity =>
            {
                entity.HasKey(e => e.ProcessAdministrativeProceduresId);

                entity.Property(e => e.ProcessAdministrativeProceduresId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Quy trình nội bộ giải quyết thủ tục hành chính - Thủ tục hành chính - Sở thanh tra");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ProcessAdministrativeProceduresCode)
                    .HasMaxLength(100)
                    .HasComment("Mã quy trình");

                entity.Property(e => e.ProcessAdministrativeProceduresField).HasComment("Lĩnh vực giải quyết");

                entity.Property(e => e.ProcessAdministrativeProceduresName)
                    .HasMaxLength(500)
                    .HasComment("Tên quy trình");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProcessAdministrativeProceduresStep>(entity =>
            {
                entity.ToTable("ProcessAdministrativeProcedures_Step");

                entity.Property(e => e.ProcessAdministrativeProceduresStepId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Quy trình nội bộ giải quyết thủ tục hành chính - Các bước");

                entity.Property(e => e.ContentImplementation).HasComment("Nội dung thực hiện");

                entity.Property(e => e.ImplementingAgencies)
                    .HasMaxLength(500)
                    .HasComment("Đơn vị thực hiện");

                entity.Property(e => e.ProcessAdministrativeProceduresId).HasComment("Id quy trình nội bộ giải quyết thủ tục hành chính");

                entity.Property(e => e.ProcessingTime)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Thời gian thực hiện");

                entity.Property(e => e.Step).HasComment("Bước");
            });

            modelBuilder.Entity<ProductOcop>(entity =>
            {
                entity.ToTable("ProductOCOP");

                entity.Property(e => e.ProductOcopid)
                    .HasColumnName("ProductOCOPId")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address).HasMaxLength(250);

                entity.Property(e => e.ApprovalDecision)
                    .HasMaxLength(250)
                    .HasComment("Quyết định phê duyệt");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DistrictId).HasComment("Huyện");

                entity.Property(e => e.Expiry).HasComment("Hạn sử dụng");

                entity.Property(e => e.Ingredients)
                    .HasMaxLength(250)
                    .HasComment("Thành phần");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(11)
                    .HasComment("Số điện thoại");

                entity.Property(e => e.Preserve)
                    .HasMaxLength(250)
                    .HasComment("Bảo Quản");

                entity.Property(e => e.ProductName).HasMaxLength(250);

                entity.Property(e => e.ProductOwner).HasMaxLength(250);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProductOcopAttachFile>(entity =>
            {
                entity.ToTable("ProductOCOP_AttachFile");

                entity.Property(e => e.ProductOcopattachFileId)
                    .HasColumnName("ProductOCOPAttachFileId")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.LinkFile).HasMaxLength(250);

                entity.Property(e => e.ProductOcopid).HasColumnName("ProductOCOPId");

                entity.Property(e => e.Type).HasComment("0: hình ảnh 1: file quyết định phê duyệt");
            });

            modelBuilder.Entity<ProposedPowerProject>(entity =>
            {
                entity.ToTable("ProposedPowerProject");

                entity.Property(e => e.ProposedPowerProjectId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Quản lý dự án nguồn điện đang đề xuất");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Vị trí");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.EnergyIndustryId).HasComment("Id lĩnh vực");

                entity.Property(e => e.InvestorName)
                    .HasMaxLength(500)
                    .HasComment("Nhà đầu tư");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note)
                    .HasMaxLength(500)
                    .HasComment("Ghi chú");

                entity.Property(e => e.PolicyDecision)
                    .HasMaxLength(500)
                    .HasComment("Văn bản pháp ly");

                entity.Property(e => e.ProjectName)
                    .HasMaxLength(500)
                    .HasComment("Tên dự án");

                entity.Property(e => e.StatusId).HasComment("Id trạng thái - Lấy từ config");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Wattage).HasComment("Công suất");
            });

            modelBuilder.Entity<RecordsFinancePlan>(entity =>
            {
                entity.ToTable("RecordsFinancePlan");

                entity.Property(e => e.RecordsFinancePlanId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<RecordsManager>(entity =>
            {
                entity.ToTable("RecordsManager");

                entity.Property(e => e.RecordsManagerId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CodeFile).HasMaxLength(100);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Creator).HasMaxLength(500);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.ReceptionTime).HasColumnType("datetime");

                entity.Property(e => e.StorageTime).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<RegulationConformityAm>(entity =>
            {
                entity.ToTable("RegulationConformityAM");

                entity.Property(e => e.RegulationConformityAmid)
                    .HasColumnName("RegulationConformityAMId")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .HasComment("Địa chỉ");

                entity.Property(e => e.Content)
                    .HasMaxLength(250)
                    .HasComment("Nội dung");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DateOfPublication)
                    .HasColumnType("date")
                    .HasComment("Ngày công bố");

                entity.Property(e => e.DayReception)
                    .HasColumnType("date")
                    .HasComment("Ngày tiếp nhận");

                entity.Property(e => e.DistrictId).HasComment("Id huyện");

                entity.Property(e => e.EstablishmentId).HasComment("Tên cơ sở");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note)
                    .HasMaxLength(250)
                    .HasComment("Ghi chú");

                entity.Property(e => e.Num)
                    .HasMaxLength(50)
                    .HasComment("Số công bố");

                entity.Property(e => e.Phone)
                    .HasMaxLength(11)
                    .HasComment("Số điện thoại");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(50)
                    .HasComment("Tên sản phẩm");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<RegulationConformityAmLog>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.ToTable("RegulationConformityAM_Log");

                entity.Property(e => e.LogId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.ItemId).HasComment("Id của công bố");

                entity.Property(e => e.LogTime)
                    .HasColumnType("datetime")
                    .HasComment("Thời gian thay đổi");

                entity.Property(e => e.NewValue)
                    .HasMaxLength(50)
                    .HasComment("Giá trị mới");

                entity.Property(e => e.OldValue)
                    .HasMaxLength(50)
                    .HasComment("Giá trị cũ");

                entity.Property(e => e.Property)
                    .HasMaxLength(50)
                    .HasComment("Tên trường thay đổi");

                entity.Property(e => e.UserId).HasComment("Id người dùng thực hiện thay đổi");
            });

            modelBuilder.Entity<RegulationConformityAmProduct>(entity =>
            {
                entity.HasKey(e => e.ProductId);

                entity.ToTable("RegulationConformityAM_Product");

                entity.Property(e => e.ProductId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("ID sản phẩm");

                entity.Property(e => e.Note)
                    .HasMaxLength(500)
                    .HasComment("Ghi chú");

                entity.Property(e => e.ProductName)
                    .HasMaxLength(500)
                    .HasComment("Tên sản phẩm");

                entity.Property(e => e.RegulationConformityAmid)
                    .HasColumnName("RegulationConformityAMId")
                    .HasComment("ID Công bố hợp quy");
            });

            modelBuilder.Entity<ReportAdministrativeProcedure>(entity =>
            {
                entity.HasKey(e => e.ReportId);

                entity.Property(e => e.ReportId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng thủ báo cáo tình hình giải quyết thủ tục hành chính - thủ tục hành chính - thanh tra sở");

                entity.Property(e => e.AdministrativeProceduresField).HasComment("Id lĩnh vực");

                entity.Property(e => e.BeforeDeadlineProcessed).HasComment("Trước hạn - đã giải quyết");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.FromPreviousPeriod).HasComment("Từ kỳ trước");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.OfflineInPeriod).HasComment("Offline trong kỳ");

                entity.Property(e => e.OnTimeProcessed).HasComment("Đúng hạn - đã giải quyết");

                entity.Property(e => e.OnTimeProcessing).HasComment("Trong hạng - đang xử lý");

                entity.Property(e => e.OnlineInPeriod).HasComment("Online trong kỳ");

                entity.Property(e => e.OutOfDateProcessed).HasComment("Quá hạn - đã giải quyết");

                entity.Property(e => e.OutOfDateProcessing).HasComment("Quá hạn - đang xử lý");

                entity.Property(e => e.Period).HasComment("Kỳ báo cáo");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Year).HasComment("Năm báo cáo");
            });

            modelBuilder.Entity<ReportIndexIndustry>(entity =>
            {
                entity.ToTable("ReportIndexIndustry");

                entity.Property(e => e.ReportIndexIndustryId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ReportIndex).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ReportIndustrialCluster>(entity =>
            {
                entity.HasKey(e => e.ReportIndustrialClustersId);

                entity.Property(e => e.ReportIndustrialClustersId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Criteria).HasComment("Chỉ tiêu, lấy từ bảng CateCriteria");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.ReportingPeriod).HasComment("Kỳ báo cáo, 1: 6 tháng, 2: 1 năm");

                entity.Property(e => e.TypeReport).HasComment("1: Phương án phát triển cụm công nghiệp\r\n2: Thành lập, đầu tư xây dựng hạ tầng kỹ thuật cụm công nghiệp\r\n3: Hoạt động của các cụm công nghiệp");

                entity.Property(e => e.Units)
                    .HasMaxLength(100)
                    .HasComment("đơn vị tính");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Year).HasDefaultValueSql("((2023))");
            });

            modelBuilder.Entity<ReportOperationalStatusOfConstructionInvestmentProject>(entity =>
            {
                entity.HasKey(e => e.ReportOperationalStatusOfConstructionInvestmentProjectsId);

                entity.Property(e => e.ReportOperationalStatusOfConstructionInvestmentProjectsId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Criteria).HasComment("Chỉ tiêu, lấy từ bảng CateCriteria");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.ReportingPeriod).HasComment("Kỳ báo cáo, 1: 6 tháng, 2: 1 năm");

                entity.Property(e => e.Units)
                    .HasMaxLength(100)
                    .HasComment("đơn vị tính");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Year).HasDefaultValueSql("((2023))");
            });

            modelBuilder.Entity<ReportOperationalStatusOfInvestmentProject>(entity =>
            {
                entity.HasKey(e => e.ReportOperationalStatusOfInvestmentProjectsId);

                entity.Property(e => e.ReportOperationalStatusOfInvestmentProjectsId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Criteria).HasComment("Chỉ tiêu, lấy từ bảng CateCriteria");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.ReportingPeriod).HasComment("Kỳ báo cáo, 1: 6 tháng, 2: 1 năm");

                entity.Property(e => e.Units)
                    .HasMaxLength(100)
                    .HasComment("đơn vị tính");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Year).HasDefaultValueSql("((2023))");
            });

            modelBuilder.Entity<ReportPromotionCommerce>(entity =>
            {
                entity.ToTable("ReportPromotionCommerce");

                entity.Property(e => e.ReportPromotionCommerceId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Chief)
                    .HasMaxLength(250)
                    .HasComment("thủ trưởng đơn vị");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.Fax).HasMaxLength(250);

                entity.Property(e => e.Host)
                    .HasMaxLength(250)
                    .HasComment("đơn vị chủ trì");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Location)
                    .HasMaxLength(250)
                    .HasComment("địa điểm");

                entity.Property(e => e.Organize)
                    .HasMaxLength(250)
                    .HasComment("đơn vị tổ chức");

                entity.Property(e => e.PhoneNumber).HasMaxLength(250);

                entity.Property(e => e.Position).HasMaxLength(250);

                entity.Property(e => e.ProjectName).HasMaxLength(250);

                entity.Property(e => e.Represent).HasMaxLength(250);

                entity.Property(e => e.Scale)
                    .HasMaxLength(250)
                    .HasComment("quy mô");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<ResultsIndustrialPromotionVotingRp>(entity =>
            {
                entity.HasKey(e => e.ResultsIndustrialPromotionVotingId);

                entity.ToTable("ResultsIndustrialPromotionVotingRp");

                entity.Property(e => e.ResultsIndustrialPromotionVotingId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Targets).HasComment("Chỉ tiêu");

                entity.Property(e => e.Unit)
                    .HasMaxLength(50)
                    .HasComment("Đơn vị tính");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<RooftopSolarProjectManagement>(entity =>
            {
                entity.ToTable("RooftopSolarProjectManagement");

                entity.Property(e => e.RooftopSolarProjectManagementId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Quản lý dự án điện mặt trời áp mái");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Vị trí");

                entity.Property(e => e.Area)
                    .HasColumnType("decimal(18, 2)")
                    .HasComment("Diện tích");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.InvestorName)
                    .HasMaxLength(500)
                    .HasComment("Chủ đầu tư");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.OperationDay).HasColumnType("datetime");

                entity.Property(e => e.Progress)
                    .HasMaxLength(500)
                    .HasComment("Tiến độ");

                entity.Property(e => e.ProjectName)
                    .HasMaxLength(500)
                    .HasComment("Tên dự án");

                entity.Property(e => e.SurveyPolicy)
                    .HasMaxLength(500)
                    .HasComment("Chủ trương khảo");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Wattage)
                    .HasColumnType("decimal(18, 4)")
                    .HasComment("Công suất");
            });

            modelBuilder.Entity<RuralDevelopmentPlan>(entity =>
            {
                entity.ToTable("RuralDevelopmentPlan");

                entity.Property(e => e.RuralDevelopmentPlanId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Kế hoạch phát triển chợ nông thôn");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ");

                entity.Property(e => e.Budget).HasComment("Ngân sách");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.OutOfBudget).HasComment("Ngoài ngân sách");

                entity.Property(e => e.StageId).HasComment("///Giai đoạn");

                entity.Property(e => e.SuperMarketShoppingMallName)
                    .HasMaxLength(500)
                    .HasColumnName("SuperMarket_ShoppingMall_Name")
                    .HasComment("Tên TTTM / Siêu thị");

                entity.Property(e => e.TotalInvestment).HasComment("Tổng vốn đầu tư");

                entity.Property(e => e.Type).HasComment("Loại hình: 0 - Xây dựng, 1 - Nâng cấp");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<RuralDevelopmentPlanStage>(entity =>
            {
                entity.HasKey(e => e.PlanStageId);

                entity.ToTable("RuralDevelopmentPlan_Stage");

                entity.Property(e => e.PlanStageId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Giai đoạn");

                entity.Property(e => e.Budget).HasComment("Năm kết thúc");

                entity.Property(e => e.StageId).HasComment("Tên giai đoạn");

                entity.Property(e => e.Year).HasComment("Năm bắt đầu");
            });

            modelBuilder.Entity<Sample>(entity =>
            {
                entity.ToTable("Sample");

                entity.Property(e => e.SampleId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.SampleCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.SampleName).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<SampleContract>(entity =>
            {
                entity.ToTable("SampleContract");

                entity.Property(e => e.SampleContractId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng hợp đồng mẫu - Sở thanh tra");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Địa chỉ");

                entity.Property(e => e.BusinessName)
                    .HasMaxLength(500)
                    .HasDefaultValueSql("('')")
                    .HasComment("Tên cơ quan/ tổ chức");

                entity.Property(e => e.BusinessPhoneNumber)
                    .HasMaxLength(20)
                    .HasComment("Số điện thoại cơ quan / tổ chức");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .HasComment("Số điện thoại liên hệ");

                entity.Property(e => e.ProfileNumber)
                    .HasMaxLength(500)
                    .HasComment("Số hồ sơ");

                entity.Property(e => e.RegistrantName)
                    .HasMaxLength(500)
                    .HasComment("Tên người đăng ký");

                entity.Property(e => e.RegistrationTime)
                    .HasColumnType("datetime")
                    .HasComment("Thời gian đăng ký");

                entity.Property(e => e.SampleContractField).HasComment("Lĩnh vực giải quyết");

                entity.Property(e => e.TaxCode)
                    .HasMaxLength(100)
                    .HasComment("Mã số thuế");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Stage>(entity =>
            {
                entity.ToTable("Stage");

                entity.Property(e => e.StageId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Giai đoạn");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.EndYear).HasComment("Năm kết thúc");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.StageName)
                    .HasMaxLength(500)
                    .HasComment("Tên giai đoạn");

                entity.Property(e => e.StartYear).HasComment("Năm bắt đầu");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<StateTitle>(entity =>
            {
                entity.HasKey(e => e.StateTitlesId);

                entity.Property(e => e.StateTitlesId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.StateTitlesCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StateTitlesName).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<StateUnit>(entity =>
            {
                entity.HasKey(e => e.StateUnitsId)
                    .HasName("PK_Units");

                entity.Property(e => e.StateUnitsId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng đơn vị");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ParentId).HasComment("Id đơn vị cha");

                entity.Property(e => e.StateUnitsCode)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("Mã đơn vị");

                entity.Property(e => e.StateUnitsName)
                    .HasMaxLength(500)
                    .HasComment("Tên đơn vị");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<SubMenu>(entity =>
            {
                entity.HasKey(e => e.IdSub);

                entity.ToTable("SubMenu");

                entity.Property(e => e.AllowPermit).HasMaxLength(500);

                entity.Property(e => e.Icon).HasMaxLength(500);

                entity.Property(e => e.IdMainMenu).HasMaxLength(500);

                entity.Property(e => e.Link).HasMaxLength(500);

                entity.Property(e => e.PageKey).HasMaxLength(500);

                entity.Property(e => e.Summary).HasMaxLength(500);

                entity.Property(e => e.Target).HasMaxLength(500);

                entity.Property(e => e.Title).HasMaxLength(500);
            });

            modelBuilder.Entity<SysColumn>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Sys_Columns");

                entity.Property(e => e.ColumnKey)
                    .HasMaxLength(200)
                    .HasColumnName("column_key");

                entity.Property(e => e.ColumnName)
                    .HasMaxLength(200)
                    .HasColumnName("column_name");

                entity.Property(e => e.DataType)
                    .HasMaxLength(100)
                    .HasColumnName("data_type");

                entity.Property(e => e.IsExist)
                    .HasColumnName("is_exist")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsNull)
                    .HasColumnName("is_null")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.PrimaryRefId)
                    .HasMaxLength(100)
                    .HasColumnName("primary_ref_id");

                entity.Property(e => e.RefId)
                    .HasMaxLength(100)
                    .HasColumnName("ref_id");

                entity.Property(e => e.RefName)
                    .HasMaxLength(200)
                    .HasColumnName("ref_name");

                entity.Property(e => e.RefTable)
                    .HasMaxLength(100)
                    .HasColumnName("ref_table");

                entity.Property(e => e.TableKey)
                    .HasMaxLength(100)
                    .HasColumnName("table_key");
            });

            modelBuilder.Entity<SysTable>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Sys_Table");

                entity.Property(e => e.TableKey)
                    .HasMaxLength(100)
                    .HasColumnName("table_key");

                entity.Property(e => e.TableName)
                    .HasMaxLength(500)
                    .HasColumnName("table_name");

                entity.Property(e => e.Url)
                    .HasMaxLength(500)
                    .HasColumnName("url");
            });

            modelBuilder.Entity<SystemLog>(entity =>
            {
                entity.HasKey(e => e.LogId);

                entity.ToTable("SystemLog");

                entity.Property(e => e.LogId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Account).HasMaxLength(200);

                entity.Property(e => e.ActionName).HasMaxLength(200);

                entity.Property(e => e.ActionType).HasMaxLength(200);

                entity.Property(e => e.ApplicationCode).HasMaxLength(200);

                entity.Property(e => e.ContentLog).HasMaxLength(1000);

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.ErrorDescription).HasMaxLength(200);

                entity.Property(e => e.IpPortCurrentNode)
                    .HasMaxLength(200)
                    .HasColumnName("IP_Port_CurrentNode");

                entity.Property(e => e.IpPortParentNode)
                    .HasMaxLength(200)
                    .HasColumnName("IP_Port_ParentNode");

                entity.Property(e => e.ReponseConent).HasMaxLength(1000);

                entity.Property(e => e.RequestConent).HasMaxLength(1000);

                entity.Property(e => e.ServiceCode).HasMaxLength(200);

                entity.Property(e => e.SessionId)
                    .HasMaxLength(100)
                    .HasColumnName("SessionID");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.TransactionStatus).HasMaxLength(200);

                entity.Property(e => e.UserName).HasMaxLength(200);
            });

            modelBuilder.Entity<TableResult>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TableResult");
            });

            modelBuilder.Entity<Target1708>(entity =>
            {
                entity.ToTable("Target1708");

                entity.Property(e => e.Target1708Id)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng tiêu chí 17.08");

                entity.Property(e => e.CommuneId).HasComment("Mã xã lấy từ bảng Commune");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DistrictId).HasComment("ID Huyện lấy từ bảng District");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.NewRuralCriteria).HasComment("Đạt tiêu chí nông thôn mới");

                entity.Property(e => e.NewRuralCriteriaRaised).HasComment("Đạt tiêu chí nông thôn mới nâng cao");

                entity.Property(e => e.Note)
                    .HasMaxLength(500)
                    .HasComment("Ghi chú");

                entity.Property(e => e.StageId).HasComment("ID giai đoạn lấy từ bảng Stage");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Target7>(entity =>
            {
                entity.ToTable("Target7");

                entity.Property(e => e.Target7Id)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng tiêu chí số 7");

                entity.Property(e => e.CommuneId).HasComment("Mã xã lấy từ bảng Commune");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DistrictId).HasComment("ID Huyện lấy từ bảng District");

                entity.Property(e => e.EstimateCommercial).HasComment("Đạt cơ sở hạ tầng thương mại nông thôn mới - Ước tính");

                entity.Property(e => e.EstimateMarket).HasComment("Chợ đạt chuẩn nông thôn mới - Ước tính");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.MarketInPlaning).HasComment("Số chợ trong quy hoạch");

                entity.Property(e => e.Note)
                    .HasMaxLength(500)
                    .HasComment("Ghi chú");

                entity.Property(e => e.PlanCommercial).HasComment("Đạt cơ sở hạ tầng thương mại nông thôn mới - Theo kế hoạch");

                entity.Property(e => e.PlanMarket).HasComment("Chợ đạt chuẩn nông thôn mới - Kế hoạch");

                entity.Property(e => e.StageId).HasComment("ID giai đoạn lấy từ bảng Config");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Year).HasComment("Năm báo cáo");
            });

            modelBuilder.Entity<TblTest>(entity =>
            {
                entity.ToTable("Tbl_Test");

                entity.Property(e => e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<TestGuidManagement>(entity =>
            {
                entity.ToTable("TestGuidManagement");

                entity.Property(e => e.TestGuidManagementId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CoordinationAgency).HasMaxLength(100);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.InspectionAgency).HasMaxLength(100);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Result).HasMaxLength(100);

                entity.Property(e => e.Time).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TestGuidManagementAttachFile>(entity =>
            {
                entity.ToTable("TestGuidManagement_AttachFile");

                entity.Property(e => e.TestGuidManagementAttachFileId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LinkFile).HasMaxLength(250);
            });

            modelBuilder.Entity<TimeManagementSeminar>(entity =>
            {
                entity.ToTable("TimeManagementSeminar");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TotalRetailSale>(entity =>
            {
                entity.ToTable("TotalRetailSale");

                entity.Property(e => e.TotalRetailSaleId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.ReportIndex).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TradeFairOrganizationCertification>(entity =>
            {
                entity.ToTable("TradeFairOrganizationCertification");

                entity.Property(e => e.TradeFairOrganizationCertificationId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Quản lý xác nhận tổ chức hội chợ triển lãm thương mại - Xúc tiến thương mại");

                entity.Property(e => e.Address)
                    .HasMaxLength(500)
                    .HasComment("Địa điểm tổ chức");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Scale)
                    .HasMaxLength(500)
                    .HasComment("Quy mô");

                entity.Property(e => e.TextNumber)
                    .HasMaxLength(500)
                    .HasComment("Số văn bản");

                entity.Property(e => e.TradeFairName)
                    .HasMaxLength(500)
                    .HasComment("Tên hội chợ / Triển lãm thương mại");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TradeFairOrganizationCertificationAttachFile>(entity =>
            {
                entity.HasKey(e => e.TradeFairFileId);

                entity.ToTable("TradeFairOrganizationCertification_AttachFile");

                entity.Property(e => e.TradeFairFileId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("File đính kèm văn bản xác nhận - Quản lý xác nhận tổ chức hội chợ triển lãm thương mại - Xúc tiến thương mại");

                entity.Property(e => e.LinkFile).HasMaxLength(250);
            });

            modelBuilder.Entity<TradeFairOrganizationCertificationTime>(entity =>
            {
                entity.HasKey(e => e.TradeFairTimeId);

                entity.ToTable("TradeFairOrganizationCertification_Time");

                entity.Property(e => e.TradeFairTimeId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Thời gian tổ chức  - Quản lý xác nhận tổ chức hội chợ triển lãm thương mại - Xúc tiến thương mại");

                entity.Property(e => e.EndTime)
                    .HasColumnType("datetime")
                    .HasComment("Thời gian kết thúc");

                entity.Property(e => e.StartTime)
                    .HasColumnType("datetime")
                    .HasComment("Thời gian bắt đầu");

                entity.Property(e => e.TradeFairOrganizationCertificationId).HasComment("");
            });

            modelBuilder.Entity<TradePromotionActivityReport>(entity =>
            {
                entity.ToTable("TradePromotionActivityReport");

                entity.Property(e => e.TradePromotionActivityReportId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng báo cáo hoạt động xúc tiến thương mại");

                entity.Property(e => e.Address)
                    .HasMaxLength(250)
                    .HasComment("Địa chỉ");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DistrictId).HasComment("Id huyện");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasComment("Thời gian kết thúc");

                entity.Property(e => e.FundingSupport).HasComment("Kinh phí hổ trợ");

                entity.Property(e => e.ImplementationCost).HasComment("Kinh phí thực hiện");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note)
                    .HasMaxLength(250)
                    .HasComment("Ghi chú");

                entity.Property(e => e.NumParticipating).HasComment("Số lượng doanh nghiệp tham gia");

                entity.Property(e => e.PlanName)
                    .HasMaxLength(250)
                    .HasComment("Tên đề án");

                entity.Property(e => e.PlanToJoin).HasComment("Kế hoạch tham gia");

                entity.Property(e => e.Scale)
                    .HasMaxLength(250)
                    .HasComment("Quy mô");

                entity.Property(e => e.ScaleId).HasComment("Id quy mô: 0 - Trong tỉnh, 1 - Ngoài tỉnh, 2 - Ngoài nước");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày bắt đầu");

                entity.Property(e => e.Time)
                    .HasMaxLength(250)
                    .HasComment("Thời lượng");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TradePromotionActivityReportAttachFile>(entity =>
            {
                entity.ToTable("TradePromotionActivityReport_AttachFile");

                entity.Property(e => e.TradePromotionActivityReportAttachFileId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LinkFile).HasMaxLength(250);
            });

            modelBuilder.Entity<TradePromotionActivityReportParticipatingBusiness>(entity =>
            {
                entity.HasKey(e => e.ParticipatingBusinessId);

                entity.ToTable("TradePromotionActivityReport_ParticipatingBusiness");

                entity.Property(e => e.ParticipatingBusinessId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng doanh nghiệp tham gia");

                entity.Property(e => e.Address)
                    .HasMaxLength(250)
                    .HasComment("Địa chỉ");

                entity.Property(e => e.BusinessId).HasComment("Id doanh nghiệp, Guid.Empty nếu là doanh nghiệp ngoài tỉnh");

                entity.Property(e => e.BusinessName)
                    .HasMaxLength(250)
                    .HasComment("Tên doanh nghiệp");
            });

            modelBuilder.Entity<TradePromotionOther>(entity =>
            {
                entity.ToTable("TradePromotionOther");

                entity.Property(e => e.TradePromotionOtherId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Bảng quản lý xúc tiến thương mại khác");

                entity.Property(e => e.Address)
                    .HasMaxLength(250)
                    .HasComment("Địa chỉ");

                entity.Property(e => e.Content)
                    .HasMaxLength(250)
                    .HasComment("Nội dung hoạt động");

                entity.Property(e => e.Coordination)
                    .HasMaxLength(250)
                    .HasColumnName("Coordination ")
                    .HasComment("Đơn vị kết nối");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DistrictId).HasComment("Huyện");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày kết thúc");

                entity.Property(e => e.ImplementationCost).HasComment("Kinh phí thực hiện");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note)
                    .HasMaxLength(250)
                    .HasComment("Ghi chú");

                entity.Property(e => e.Participating)
                    .HasMaxLength(250)
                    .HasComment("Đơn vị tham gia, kinh phí thực hiện");

                entity.Property(e => e.Result)
                    .HasMaxLength(250)
                    .HasComment("Kết quả");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày bắt đầu");

                entity.Property(e => e.Time)
                    .HasMaxLength(250)
                    .HasComment("Thời lượng / Số lượng, thời lượng phát sóng");

                entity.Property(e => e.TypeOfActivity).HasComment("Loại hình hoạt động");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TradePromotionOtherAttachFile>(entity =>
            {
                entity.ToTable("TradePromotionOther_AttachFile");

                entity.Property(e => e.TradePromotionOtherAttachFileId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LinkFile).HasMaxLength(250);
            });

            modelBuilder.Entity<TradePromotionProgramForBusiness>(entity =>
            {
                entity.HasKey(e => e.TradePromotionProgramBusinessId);

                entity.ToTable("TradePromotionProgramForBusiness");

                entity.Property(e => e.TradePromotionProgramBusinessId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TradePromotionProgramForBusinessDetail>(entity =>
            {
                entity.ToTable("TradePromotionProgramForBusinessDetail");

                entity.Property(e => e.TradePromotionProgramForBusinessDetailId).HasDefaultValueSql("(newid())");
            });

            modelBuilder.Entity<TradePromotionProject>(entity =>
            {
                entity.ToTable("TradePromotionProject");

                entity.Property(e => e.TradePromotionProjectId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.TradePromotionProjectCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TradePromotionProjectName).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TradePromotionProjectManagement>(entity =>
            {
                entity.ToTable("TradePromotionProjectManagement");

                entity.Property(e => e.TradePromotionProjectManagementId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Cost).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ImplementationResults).HasDefaultValueSql("((0))");

                entity.Property(e => e.ImplementingAgencies).HasMaxLength(500);

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Reason).HasMaxLength(500);

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.Property(e => e.TimeEnd).HasColumnType("datetime");

                entity.Property(e => e.TimeStart).HasColumnType("datetime");

                entity.Property(e => e.TradePromotionProjectManagementName).HasMaxLength(100);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TradePromotionProjectManagementAttachFile>(entity =>
            {
                entity.ToTable("TradePromotionProjectManagement_AttachFile");

                entity.Property(e => e.TradePromotionProjectManagementAttachFileId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LinkFile).HasMaxLength(250);
            });

            modelBuilder.Entity<TradePromotionProjectManagementBussiness>(entity =>
            {
                entity.ToTable("TradePromotionProjectManagement_Bussiness");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.BusinessCode).HasMaxLength(250);

                entity.Property(e => e.BusinessNameVi).HasMaxLength(250);

                entity.Property(e => e.DiaChi).HasMaxLength(250);

                entity.Property(e => e.NganhNghe).HasMaxLength(250);

                entity.Property(e => e.NguoiDaiDien).HasMaxLength(250);
            });

            modelBuilder.Entity<TrainingClassManagement>(entity =>
            {
                entity.ToTable("TrainingClassManagement");

                entity.Property(e => e.TrainingClassManagementId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Location).HasMaxLength(100);

                entity.Property(e => e.Participant).HasMaxLength(100);

                entity.Property(e => e.TimeStart).HasColumnType("datetime");

                entity.Property(e => e.Topic).HasMaxLength(100);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TrainingClassManagementAttachFile>(entity =>
            {
                entity.ToTable("TrainingClassManagement_AttachFile");

                entity.Property(e => e.TrainingClassManagementAttachFileId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LinkFile).HasMaxLength(250);
            });

            modelBuilder.Entity<TrainingManagement>(entity =>
            {
                entity.ToTable("TrainingManagement");

                entity.Property(e => e.TrainingManagementId)
                    .HasDefaultValueSql("(newid())")
                    .HasComment("Quản lý đào tạo tập huấn");

                entity.Property(e => e.Address)
                    .HasMaxLength(250)
                    .HasComment("Địa chỉ");

                entity.Property(e => e.Annunciator)
                    .HasMaxLength(250)
                    .HasComment("Báo cáo viên");

                entity.Property(e => e.Content)
                    .HasMaxLength(250)
                    .HasComment("Nội dung tập huấn");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DistrictId).HasComment("Huyện");

                entity.Property(e => e.EndDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày kết thúc");

                entity.Property(e => e.ImplementationCost).HasComment("Kinh phí thực hiện");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note)
                    .HasMaxLength(250)
                    .HasComment("Ghi chú");

                entity.Property(e => e.NumParticipating).HasComment("Số người tham gia");

                entity.Property(e => e.Participating)
                    .HasMaxLength(250)
                    .HasComment("Đơn vị tham gia");

                entity.Property(e => e.StartDate)
                    .HasColumnType("datetime")
                    .HasComment("Ngày bắt đầu");

                entity.Property(e => e.Time)
                    .HasMaxLength(250)
                    .HasComment("Thời lượng");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TrainingManagementAttachFile>(entity =>
            {
                entity.ToTable("TrainingManagement_AttachFile");

                entity.Property(e => e.TrainingManagementAttachFileId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LinkFile).HasMaxLength(250);
            });

            modelBuilder.Entity<TypeOfBusiness>(entity =>
            {
                entity.ToTable("TypeOfBusiness");

                entity.Property(e => e.TypeOfBusinessId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.TypeOfBusinessCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TypeOfBusinessName).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TypeOfEnergy>(entity =>
            {
                entity.ToTable("TypeOfEnergy");

                entity.Property(e => e.TypeOfEnergyId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.TypeOfEnergyCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TypeOfEnergyName).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TypeOfMarket>(entity =>
            {
                entity.ToTable("TypeOfMarket");

                entity.Property(e => e.TypeOfMarketId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.TypeOfMarketCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TypeOfMarketName).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TypeOfProfession>(entity =>
            {
                entity.ToTable("TypeOfProfession");

                entity.Property(e => e.TypeOfProfessionId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.TypeOfProfessionCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TypeOfProfessionName).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<TypeOfTradePromotion>(entity =>
            {
                entity.ToTable("TypeOfTradePromotion");

                entity.Property(e => e.TypeOfTradePromotionId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.TypeOfTradePromotionCode)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.TypeOfTradePromotionName).HasMaxLength(500);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.Property(e => e.UnitId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.Exchange).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.IsAction)
                    .IsRequired()
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsDel).HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.UnitCode)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UnitName).HasMaxLength(500);

                entity.Property(e => e.UnitNameEn)
                    .HasMaxLength(500)
                    .HasColumnName("UnitNameEN");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.UserName });

                entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.UserName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasComment("@import Tên người dùng");

                entity.Property(e => e.AreaId)
                    .HasColumnName("AreaID")
                    .HasComment("ID của xã huyện tỉnh dùng cột LevelUser để xác định bảng Join.");

                entity.Property(e => e.Avatar).HasMaxLength(1000);

                entity.Property(e => e.Cccd)
                    .HasMaxLength(50)
                    .HasColumnName("CCCD");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())")
                    .HasComment("Thời gian tạo");

                entity.Property(e => e.CreateUserId).HasComment("Người tạo");

                entity.Property(e => e.DeptId).HasColumnName("DeptID");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.FullName)
                    .HasMaxLength(500)
                    .HasComment("@Họ và tên@");

                entity.Property(e => e.GroupUserId).HasColumnName("GroupUserID");

                entity.Property(e => e.IsAction)
                    .HasDefaultValueSql("((1))")
                    .HasComment("1: Hoạt động 0: Không hoạt động");

                entity.Property(e => e.IsAdmin)
                    .HasColumnName("isAdmin")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.IsDel)
                    .HasDefaultValueSql("((0))")
                    .HasComment("1: Đã xóa; 0: Chưa xóa");

                entity.Property(e => e.LevelUser).HasComment("Cấp: 0 - Cấp Tỉnh, 1 - Cấp Huyện, 2 - Cấp Xã.");

                entity.Property(e => e.Password).HasMaxLength(100);

                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.Property(e => e.TimeLock).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(e => new { e.GroupId, e.UserName });

                entity.ToTable("UserGroup");

                entity.Property(e => e.UserName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UuiRefreshToken>(entity =>
            {
                entity.HasKey(e => new { e.Uuid, e.UserName });

                entity.ToTable("UuiRefreshToken");

                entity.Property(e => e.UserName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
