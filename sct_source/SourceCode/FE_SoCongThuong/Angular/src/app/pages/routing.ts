import { Routes } from '@angular/router';
import { Permission } from '../modules/auth/services/permission';

const Routing: Routes = [
  /**Dashboard --------------------------------------------------------------------------------------------- */
  {
    path: 'dashboard',
    loadChildren: () =>
      import('./dashboard/dashboard.module').then((m) => m.DashboardModule),
    data: {
      layout: 'light-sct-sidebar'
    }
  },
  /** ----------------------------------------------------------------------------------------- Hết Dashboard*/

  /**Quản lý người dùng ------------------------------------------------------------------------------------ */
  //Danh mục người dùng
  {
    path: 'user',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/user-page/user-page.module').then((m) => m.UserPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['2', '3', '4', '5', '6']
    },
  },
  //Quản trị nhóm người dùng
  {
    path: 'group-user',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/group-user-page/group-user-page.module').then((m) => m.GroupUserPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['2', '7', '8', '9', '10']
    },
  },
  /** -------------------------------------------------------------------------------- Hết quản lý người dùng*/

  /**Danh mục dùng chung ----------------------------------------------------------------------------------- */
  //Quản lý danh mục huyện
  {
    path: 'district',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/district-page/district-page.module').then((m) => m.DistrictPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '12', '13', '14', '15']
    },
  },
  //Quản lý danh mục xã
  {
    path: 'commune',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/commune-page/commune-page.module').then((m) => m.CommunePageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '22', '23', '24', '25']
    },
  },
  //Quản lý danh mục ngành nghề
  {
    path: 'industry',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/industry-page/industry-page.module').then((m) => m.IndustryPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '26', '27', '28', '29']
    },
  },
  //Quản lý danh mục quốc gia xuất khẩu, nhập khẩu
  {
    path: 'country',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/country-page/country-page.module').then((m) => m.CountryPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '30', '31', '32', '33']
    },
  },
  //Quản lý danh mục loại hình chợ
  {
    path: 'typeofmarket',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/typeofmarket-page/typeofmarket-page.module').then((m) => m.TypeOfMarketPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '34', '35', '36', '37']
    },
  },
  //Quản lý danh mục loại hình doanh nghiệp
  {
    path: 'typeofbusiness',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/typeofbusiness-page/typeofbusiness-page.module').then((m) => m.TypeOfBusinessPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '38', '39', '40', '41']
    },
  },
  //Quản lý danh mục lĩnh vực hoạt động ngành năng lượng
  {
    path: 'energyindustry',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/energyindustry-page/energyindustry-page.module').then((m) => m.EnergyIndustryPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '42', '43', '44', '45']
    },
  },
  //Quản lý danh mục loại hình năng lượng
  {
    path: 'typeofenergy',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/typeofenergy-page/typeofenergy-page.module').then((m) => m.TypeOfEnergyPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '46', '47', '48', '49']
    },
  },
  //Quản lý danh mục loại hình xúc tiến thương mại
  {
    path: 'typeoftradepromotion',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/typeoftradepromotion-page/typeoftradepromotion-page.module').then((m) => m.TypeOfTradePromotionPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '50', '51', '52', '53']
    },
  },
  //Quản lý danh mục đề án xúc tiến thương mại
  {
    path: 'tradepromotionproject',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/tradepromotionproject-page/tradepromotionproject-page.module').then((m) => m.TradePromotionProjectPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '54', '55', '56', '57']
    },
  },
  //Quản lý danh mục thủ tục hành chính
  {
    path: 'adminformalities',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/adminformalities-page/adminformalities-page.module').then((m) => m.AdminFormalitiesPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '58', '59', '60', '61']
    },
  },
  //Quản lý danh mục loại ngành nghề
  {
    path: 'typeofprofession',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/typeofprofession-page/typeofprofession-page.module').then((m) => m.TypeOfProfessionPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '62', '63', '64', '65']
    },
  },
  //Quản lý danh mục doanh nghiệp
  {
    path: 'business',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/business-page/business-page.module').then((m) => m.BusinessPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '297', '298', '299', '300']
    },
  },
  //Quản lý danh mục chức vụ
  {
    path: 'statetitles',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/statetitles-page/statetitles-page.module').then((m) => m.StateTitlesPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '70', '71', '72', '73']
    },
  },
  //Quản lý danh mục đơn vị
  {
    path: 'stateunits',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/stateunits-page/stateunits-page.module').then((m) => m.StateUnitsPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '74', '75', '76', '77']
    },
  },
  //Quản lý tiêu chí
  {
    path: 'catecriteria',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/cate-criteria-page/cate-criteria-page.module').then((m) => m.CateCriteriaModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '294', '295', '296', '297']
    },
  },
  //Quản lý danh mục mặt hàng
  {
    path: 'businessline',
    canActivate: [Permission],
    loadChildren: () =>
      import('../modules/app-pages/business-line-page/business-line-page.module').then((m) => m.BusinessLinePageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '430', '431', '432', '433']
    },
  },
  //Quản lý danh mục giai đoạn
  {
    path: 'Stage',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/stage-page/stage-page.module').then((m) => m.StagePageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '447', '448', '449', '450']
    },
  },
  //Lĩnh vực giải quyết - thủ tục hành chính
  {
    path: 'AdministrativeProcedureField',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/administrative-procedure-field-page/administrative-procedure-field-page.module').then((m) => m.AdministrativeProcedureFieldModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '471', '472', '473', '474']
    },
  },
  //Quản lý danh mục đơn vị tính
  {
    path: 'units',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/units-page/units-page.module').then((m) => m.UnitsPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '74', '75', '76', '77']
    },
  },
  // Quản lý danh mục nhóm hồ sơ KHTC
  {
    path: 'RecordsFinancePlan',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/records-finance-plan-page/records-finance-plan-page.module').then((m) => m.RecordsFinancePlanPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '475', '476', '477', '478']
    },
  },
  // Danh mục Lĩnh vực kinh doanh khí
  {
    path: 'Gas',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/gas-page/gas-page.module').then((m) => m.GasPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '479', '480', '481', '482']
    },
  },
  /** ------------------------------------------------------------------------------- Hết danh mục dùng chung*/

  /**Phòng thương mại -------------------------------------------------------------------------------------- */
  //Quản lý thông tin chợ, siêu thị, trung tâm thương mại
  {
    path: 'commercialmanagement',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/commercialmanagement-page/commercialmanagement-page.module').then((m) => m.CommercialManagementPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '82',
        '83', '84', '85', '86',
        '91', '92', '93', '94',
        '95', '96', '97', '98']
    },
  },
  //Kế hoạch phát triển chợ nông thôn - Quản lý cơ sở hạ tầng thương mại - Phòng thương mại
  {
    path: 'RuralDevelopmentPlan',
    canActivate: [Permission],
    loadChildren: () =>
      import('../modules/app-pages/rural-development-plan-page/rural-development-plan-page.module').then((m) => m.RuralDevelopmentPlanPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '99', '100', '101', '102']
    },
  },
  // Thống kê thông tin chợ, siêu thị, trung tâm thương mại - tỉnh
  {
    path: 'ProvincialTradeStatistics',
    canActivate: [Permission],
    loadChildren: () =>
      import('../modules/app-pages/provincial-trade-statistics-page/provincial-trade-statistics-page.module').then((m) => m.ProvincialTradeStatisticsPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '413', '414', '415', '416', '417']
    },
  },
  // Thống kê thông tin chợ, siêu thị, trung tâm thương mại - huyện
  {
    path: 'DistrictTradeStatistics',
    canActivate: [Permission],
    loadChildren: () =>
      import('../modules/app-pages/district-trade-statistics-page/district-trade-statistics-page.module').then((m) => m.DistrictTradeStatisticsPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '413', '418', '419', '420', '421']
    },
  },
  //Thống kê xây dựng, nâng cấp chợ, siêu thị, trung tâm thương mại - Tỉnh
  {
    path: 'StatisticsBuildUpgrade',
    canActivate: [Permission],
    loadChildren: () =>
      import('../modules/app-pages/statistical-buildupgrade-page/statistical-buildupgrade-page.module').then((m) => m.StatisticalBuildUpgradePageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '413', '422', '423', '424', '425']
    },
  },
  //Thống kê số lượng chợ, siêu thị, trung tâm thương mại - Huyện
  {
    path: 'StatisticsMarket',
    canActivate: [Permission],
    loadChildren: () =>
      import('../modules/app-pages/statistical-market-page/statistical-market-page.module').then((m) => m.StatiscialMarketPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '413', '426', '427', '428', '429']
    },
  },
  //Quản lý đơn vị kinh doanh xăng dầu
  {
    path: 'petroleum',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/petroleum-business-page/petroleum-business-page.module').then((m) => m.PetroleumBusinessModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '103', '104', '105', '106', '107']
    },
  },
  //Quản lý đơn vị kinh doanh thuốc lá
  {
    path: 'cigarette',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/cigarette-bus-page/cigarette-bus-page.module').then((m) => m.CigaretteBusinessModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '103', '108', '109', '110', '111']
    },
  },
  //Quản lý đơn vị kinh doanh rượu
  {
    path: 'alcohol',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/alcohol-bus-page/alcohol-bus-page.module').then((m) => m.AlcoholBusinessModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '103', '112', '113', '114', '115']
    },
  },
  //Quản lý thương mại quốc tế
  {
    path: 'intercommerce',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/inter-commerce-page/inter-commerce-page.module').then((m) => m.InterCommerceModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '103', '116', '117', '118', '119']
    },
  },
  //Thống kê xăng dầu, thuốc lá, rượu - Thống kê cửa hàng kinh doanh
  {
    path: 'StatisticsBusinessStore',
    canActivate: [Permission],
    loadChildren: () =>
      import('../modules/app-pages/statistical-petroleum-page/statistical-petroleum-page.module').then((m) => m.StatiscialPetroleumPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '103', '443', '444', '445', '446']
    },
  },
  //Quản lý thông tin xuất nhập khẩu
  {
    path: 'importexport',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/importexport-page/importexport-page.module').then((m) => m.ImportExportPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '120',
        '121', '122', '123', '124',
        '125', '126', '127', '128',
        '129', '130', '131', '132']
    },
  },
  //Thống kê xuất - nhập khẩu - Tỉnh
  {
    path: 'StatisticalImportExportProvincial',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/statistical-importexport-provincial-page/statistical-importexport-provincial-page.module').then((m) => m.StatisticalImportExportProvincialPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '434', '435', '436', '437', '438']
    },
  },
  //Thống kê xuất - nhập khẩu - Huyện
  {
    path: 'StatisticalImportExportDistrict',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/statistical-importexport-district-page/statistical-importexport-district-page.module').then((m) => m.StatisticalImportExportDistrictPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '434', '439', '440', '441', '442']
    },
  },
  //Quản lý tổng mức bán lẻ hàng hóa
  // {
  //   path: 'cateretail',
  //   canActivate: [Permission],
  //   loadChildren: () =>
  //     import('..//modules/app-pages/cate-retail-page/cate-retail-page.module').then((m) => m.CateRetailModule),
  //   data: {
  //     layout: 'light-sct-sidebar',
  //     roles: ['17', '483', '133', '134', '135', '136']
  //   },
  // },
  //Quản lý doanh thu dịch vụ tiêu dùng
  {
    path: 'ConsumerServiceRevenue',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/consumer-service-revenue-page/consumer-service-revenue-page.module').then((m) => m.ConsumerServiceRevenueModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '483', '484', '485', '486', '487']
    },
  },
  //Tiêu chí số 7
  // {
  //   path: 'catecriterianumberseven',
  //   canActivate: [Permission],
  //   loadChildren: () =>
  //     import('..//modules/app-pages/cate-criteria-numbers-page/cate-criteria-page.module').then((m) => m.CateCriteriaModule),
  //   data: {
  //     layout: 'light-sct-sidebar',
  //     roles: ['17', '137', '138', '139', '140']
  //   },
  // },
  //Quản lý đề án xúc tiến thương mại
  {
    path: 'TradePromotionProjectManagement',
    canActivate: [Permission],
    loadChildren: () =>
      import('../modules/app-pages/trade-promotion-project-management-page/tradepromotionPM-page.module').then((m) => m.TradePromotionProjectManagementPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['141', '142', '143', '144', '145']
    },
  },
  //Quản lý xác nhận tổ chức hội chợ triển lãm thương mại
  {
    path: 'TradeFairOrganizationCertification',
    canActivate: [Permission],
    loadChildren: () =>
      import('../modules/app-pages/trade-fair-organization-page/trade-fair-organization-page.module').then((m) => m.TradeFairOrganizationPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['141', '146', '147', '148', '149']
    },
  },
  //Quản lý xác nhận chương trình khuyến mãi
  {
    path: 'ManageConfirmPromotion',
    canActivate: [Permission],
    loadChildren: () =>
      import('../modules/app-pages/manage-confirm-promotion-page/manage-confirm-promotion-page.module').then((m) => m.ManageConfirmPromotionPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['141', '150', '151', '152', '153']
    },
  },
  //Thống kê tình hình kinh doanh rượu
  {
    path: 'StatisticsAlcoholBusiness',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/statistical-alcoholbusiness-page/statistical-alcoholbusiness-page.module').then((m) => m.StatisticalAlcoholBusinessPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['154', '155', '459', '460', '461', '462']
    },
  },
  /** ---------------------------------------------------------------------------------- Hết thòng thương mại*/

  /**Phòng quản lý công nghiệp ----------------------------------------------------------------------------- */
  //Quản lý hộ gia đình, cá nhân sản xuất rượu
  {
    path: 'ManagerFamiliesVsPeopleProducingAlcohol',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/mnfvspproducingalcohol-page/mnfvspproducingalcohol-page.module').then((m) => m.MnFvsPProducingAlcoholPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['154', '155',
        '156', '157', '158', '159',
        '160', '161', '162', '163',
        '164', '165', '166', '167',
        '168', '169', '170', '171',
        '172', '173', '174', '175']
    },
  },
  //Thống kê tình hình sản xuất rượu thủ công / công nghiệp
  {
    path: 'StatisticalProductAlcol',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/statistical-product-alcol-page/statistical-product-alcol-page.module').then((m) => m.StatisticalProductAlcolPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['154', '155', '451', '452', '453', '454']
    },
  },
  //Quản lý số liệu doanh nghiệp công ty sản xuất công nghiệp trên địa bàn tỉnh
  {
    path: 'CateManageAncolLocalBussines',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/catemanageancollocalbussines-page/catemanageancollocalbussines-page.module').then((m) => m.CateManageAncolLocalBussinesPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['154', '176', '177', '178', '179']
    },
  },
  //Quản lý danh sách dự án
  {
    path: 'cateproject',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/cate-project-page/cate-project-page.module').then((m) => m.CateProjectModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['154', '180', '181', '182', '183', '184']
    },
  },
  //Quản lý tổng hợp
  {
    path: 'CateIntegratedManagement',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/cateintegratedmanagement-page/cateintegratedmanagement-page.module').then((m) => m.CateIntegratedManagementPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['154', '180', '185', '286', '287', '288']
    },
  },
  //Báo cáo kết quả bình chọn sản phẩm công nghiệp nông thôn tiêu biểu
  {
    path: 'resultsIndustrialPromotionVoting',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/industrial-promotion-voting-page/industrial-promotion-voting-page.module').then((m) => m.ResultsIndustrialPromotionVotingPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['154', '186', '187', '188', '189', '190']
    },
  },
  //Báo cáo kết quả công tác khuyến công
  {
    path: 'industrialPromotionResults',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/industrial-promotion-results-page/industrial-promotion-results-page.module').then((m) => m.IndustrialPromotionResultsPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['154', '186', '191', '192', '193', '194']
    },
  },
  //Báo cáo kinh phí khuyến công
  {
    path: 'industrialPromotionFunding',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/industry-promotion-report-page/industry-promotion-report-page.module').then((m) => m.IndustryPromotionReportPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['154', '186', '195', '196', '197', '198']
    },
  },
  //Quản lý thông tin cụm doanh nghiệp
  {
    path: 'IndustrialClusterInfoManagement',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/industrial-cluster-info-management-page/industrial-cluster-info-management-page.module').then((m) => m.IndustrialClusterInfoManagementPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['154', '199',
        '200', '201', '202', '203',
        '204', '205', '206', '207']
    },
  },
  //báo cáo tình hình hoạt động của dự án đầu tư trong cụm công nghiệp
  {
    path: 'RPinvestment',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/RP-of-investment-page/RP-of-investment-page.module').then((m) => m.RPOSOfInvestmentnsModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['154', '199', '208', '209', '210', '211']
    },
  },
  //báo cáo tình hình hoạt động của dự án đầu tư xây dựng hạ tầng kỹ thuật cụm công nghiệp
  {
    path: 'RPofcons',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/RP-of-con-page/RP-of-con-page.module').then((m) => m.RPOSOfConstructionsModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['154', '199', '212', '213', '214', '215']
    },
  },
  //báo cáo tình hình hoạt động của dự án đầu tư xây dựng hạ tầng kỹ thuật cụm công nghiệp
  {
    path: 'RPindustrialcluter',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/RP-industrial-page/RP-industrial-page.module').then((m) => m.RPIndustrialsModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['154', '199', '216', '217', '218', '219']
    },
  },
  /** ------------------------------------------------------------------------- Hết phòng quản lý công nghiệp*/

  /**Thanh tra sở ------------------------------------------------------------------------------------------ */
  //Thủ tục hành chính
  {
    path: 'AdministrativeProcedures',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/administrative-procedures-page/administrative-procedures-page.module').then((m) => m.AdministrativeProceduresPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['220', '285', '221', '222', '223', '224']
    },
  },
  //Quy trình nội bộ giải quyết thủ tục hành chính
  {
    path: 'ProcessAdministrativeProcedures',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/process-administrative-procedures-page/process-administrative-procedures-page.module').then((m) => m.ProcessAdministrativeProceduresPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['220', '285', '225', '226', '227', '228']
    },
  },
  //Báo cáo tình hình giải quyết thủ tục hành chính
  {
    path: 'ReportAdministrativeProcedures',
    canActivate: [Permission],
    loadChildren: () =>
      import('../modules/app-pages/report-administrative-procedures-page/report-administrative-procedures-page.module').then((m) => m.ReportAdministrativeProceduresPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['220', '285', '229', '230', '231', '232']
    },
  },
  //Quản lý cơ sở hoạt động bán hàng đa cấp
  {
    path: 'MultiLevelSalesManagement',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/multi-level-sales-management-page/multi-level-sales-management-page.module').then((m) => m.MultiLevelSalesManagementPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['220', '233', '234', '235', '236', '237']
    },
  },
  //Quản lý người tham gia bán hàng đa cấp
  {
    path: 'MultiLevelSalesParticipants',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/multi-level-sales-participants-page/multi-level-sales-participants-page.module').then((m) => m.MultiLevelSalesParticipantsPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['220', '233', '238', '239', '240', '241']
    },
  },
  //Quản lý hợp đồng mẫu
  {
    path: 'SampleContract',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/sample-contract-page/sample-contract-page.module').then((m) => m.SampleContractPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['220', '242', '243', '244', '245']
    },
  },
  //Thống kê cơ sở hoạt động bán hàng đa cấp
  {
    path: 'StatisticalMultiLevelSales',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/statistical-multi-level-sales-page/statistical-multi-level-sales-page.module').then((m) => m.StatisticalMultiLevelSalesPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['220', '233', '455', '456', '457', '458']
    },
  },
  /** -------------------------------------------------------------------------------------- Hết thanh tra sở*/

  /**Trung tâm khuyến công & xúc tiến thương mại ----------------------------------------------------------- */
  //Quản lý danh sách tham gia và hỗ trợ hội chợ
  {
    path: 'ParticipateSupportFair',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/participate-support-fair-page/participate-support-fair-page.module').then((m) => m.ParticipateSupportFairPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['256', '257', '258', '259', '260']
    },
  },
  //Quản lý báo cáo dự án khuyến công
  {
    path: 'IndustrialPromotionProject',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/industrial-promotion-project-page/industrial-promotion-project-page.module').then((m) => m.IndustrialPromotionProjectPageModule),

    data: {
      layout: 'light-sct-sidebar',
      roles: ['256', '261', '262', '263', '264']
    },
  },
  //Quản lý báo cáo hoạt động xúc tiến thương mại ReportPromotionCommerce
  {
    path: 'TradePromotionActivityReport',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/trade-promotion-activity-report-page/trade-promotion-activity-report-page.module').then((m) => m.TradePromotionActivityReportPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['256', '265', '266', '267', '268']
    },
  },
  //Quản lý doanh nghiệp tham gia chương trình xúc tiến thương mại
  // {
  //   path: 'ParticipateTradePromotion',
  //   canActivate: [Permission],
  //   loadChildren: () =>
  //     import('..//modules/app-pages/participate-trade-promotion-page/participate-trade-promotion.module').then((m) => m.ParticipateTradePromotionModule),
  //   data: {
  //     layout: 'light-sct-sidebar',
  //     roles: ['256', '269', '270', '271', '272']
  //   },
  // },
  //Báo cáo sản phẩm đạt OCOP
  {
    path: 'ProductOCOP',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/product-ocop-page/product-ocop-page.module').then((m) => m.ProductOcopPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['256', '273', '274', '275', '276']
    },
  },
  //Quản lý đào tạo tập huấn
  {
    path: 'TrainingManagement',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/training-management-page/training-management-page.module').then((m) => m.TrainingManagementPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['256', '277', '278', '279', '280']
    },
  },
  //Quản lý xúc tiến thương mại khác
  {
    path: 'TradePromotionOther',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/trade-promotion-other-page/trade-promotion-other-page.module').then((m) => m.TradePromotionOthersModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['256', '281', '282', '283', '284']
    },
  },
  //Thống kê danh sách tham gia hội chợ StatisticalFairParticipant
  {
    path: 'ParticipateTradePromotion',
    canActivate: [Permission],
    loadChildren: () =>
      //import('..//modules/app-pages/statistical-fair-participant-page/statistical-fair-participant-page.module').then((m) => m.StatisticalFairParticipantPageModule),
      import('..//modules/app-pages/participate-trade-promotion-page/participate-trade-promotion.module').then((m) => m.ParticipateTradePromotionModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['256', '463', '464', '465', '466']
    },
  },
  /** ------------------------------------------------------- Hết trung tâm khuyến công & xúc tiến thương mại*/

  /**Phòng kế hoạch tài chính ------------------------------------------------------------------------------ */
  //Các chỉ tiêu kinh doanh sản xuất, xuất/nhập khẩu chủ yếu
  {
    path: 'FinancialPlanTargets',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/financial-plan-targets-page/financial-plan-targets-page.module').then((m) => m.FinancialPlanTargetsPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['305', '315', '316', '317', '318']
    },
  },
  //Quản lý báo cáo chỉ số sản xuất công nghiệp
  {
    path: 'ReportIndexIndustry',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/report-index-industry-page/report-index-industry-page.module').then((m) => m.ReportIndexIndustryPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['305', '307', '308', '309', '310']
    },
  },
  // Quản lý hồ sơ
  {
    path: 'RecordsManager',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/records-manager-page/records-manager-page.module').then((m) => m.RecordsManagerPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['305', '311', '312', '313', '314']
    },
  },
  //Tổng mức bán lẻ hàng hóa
  {
    path: 'TotalRetailSale',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/total-retail-sale-page/total-retail-sale-page.module').then((m) => m.TotalRetailSalePageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['305', '319', '320', '321', '322']
    },
  },
  /** -------------------------------------------------------------------------- Hết phòng kế hoạch tài chính*/

  /**Phòng kỹ thuật an toàn môi trường --------------------------------------------------------------------- */
  //Quản lý cấp giấy chứng nhận - an toàn thực phẩm
  {
    path: 'FoodSafetyCertificate',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/food-safety-certificate-page/food-safety-certificate-page.module').then((m) => m.FoodSafetyCertificatePageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['323', '324', '325', '326', '327', '328']
    },
  },
  //Quản lý công bố hợp quy
  {
    path: 'RegulationConformityAnnouncementManagement',
    canActivate: [Permission],
    loadChildren: () =>
      import('../modules/app-pages/reg-conform-am-page/reg-conform-AM-page.module').then((m) => m.RegulationConformityAMPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['323', '324', '333', '334', '335', '336']
    },
  },
  //Quản lý cam kết: sản xuất, kinh doanh, vừa sản xuất vừa kinh doanh
  {
    path: 'CommitManager',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/commit-manager-page/commit-manager-page.module').then((m) => m.CommitManagerPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['323', '324', '337', '338', '339', '340']
    }
  },
  //Quản lý lớp tập huấn
  {
    path: 'TrainingClassManagement',
    canActivate: [Permission],
    loadChildren: () =>
      import('../modules/app-pages/training-class-management-page/trainclassmanage-page.module').then((m) => m.TrainingClassManagementPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['324', '341', '342', '343', '344', '345']
    },
  },
  //Quản lý kiểm tra hướng dẫn
  {
    path: 'TestGuidManagement',
    canActivate: [Permission],
    loadChildren: () =>
      import('../modules/app-pages/test-guid-management-page/testguidmanage-page.module').then((m) => m.TestGuidManagementPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['324', '341', '346', '347', '348', '349']
    },
  },
  //Quản lý đề án bảo vệ môi trương - Bảo vệ môi trường - Phòng kỹ thuật an toàn môi trường
  {
    path: 'EnvironmentProjectManagement',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/environment-project-management-page/environment-project-management-page.module').then((m) => m.EnvironmentProjectManagementPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['324', '341', '350', '351', '352', '353']
    },
  },
  //Quản lý cấp giấy chứng nhận - an toàn hóa chất
  {
    path: 'ChemicalSafetyCertificate',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/chemical-safety-certificate-page/chemical-safety-certificate-page.module').then((m) => m.ChemicalSafetyCertificatePageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['323', '354', '355', '356', '357', '358']
    },
  },
  //Quản lý doanh nghiệp hoá chất - Quản lý an toàn hoá chất - Phòng kỹ thuật an toàn môi trường
  {
    path: 'ChemicalBusinessManagement',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/chemical-business-management-page/chemical-business-management-page.module').then((m) => m.ChemicalBusinessManagementPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['323', '354', '359', '360', '361', '362']
    },
  },
  //Quản lý công tác phòng chống cháy nổ thuộc ngành công thương - Phòng kỹ thuật an toàn môi trường
  {
    path: 'ManagementFirePrevention',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/management-fire-prevention-page/management-fire-prevention-page.module').then((m) => m.ManagementFirePreventionPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['323', '363', '364', '365', '366']
    },
  },
  //Quản lý lớp tập huấn
  {
    path: 'GasTrainingClass',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/gas-training-class-management-page/trainclassmanage-page.module').then((m) => m.GasTrainingClassManagementPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['493', '323', '494', '495', '496', '497']
    },
  },
  //Quản lý lĩnh vực kinh doanh khí
  {
    path: 'GasBusiness',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/gas-business-page/gas-business-page.module').then((m) => m.GasBusinessPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['493', '323', '367', '368', '369', '370']
    },
  },
  //Quản lý hồ sơ lưu trữ
  {
    path: 'ManageArchiveRecords',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/manage-archive-records-page/manage-archive-records-page.module').then((m) => m.ManageArchiveRecordsPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['323', '371', '372', '373', '374']
    },
  },
  /** ----------------------------------------------------------------- Hết phòng kỹ thuật an toàn môi trường*/

  /**Phòng quản lý năng lượng ------------------------------------------------------------------------------ */
  //Danh sách thẻ kiểm tra viên điện lực - Phòng quản lý năng lượng
  // {
  //   path: 'ElectricityInspectorCard',
  //   canActivate: [Permission],
  //   loadChildren: () =>
  //     import('..//modules/app-pages/electricity-inspector-card-page/electricity-inspector-card-page.module').then((m) => m.ElectricityInspectorCardPageModule),
  //   data: {
  //     layout: 'light-sct-sidebar',
  //     roles: ['375', '376', '377', '378', '379']
  //   },
  // },
  //Quản lý dự án nguồn điện được phê duyệt - Các dự án nguồn điện - Phòng quản lý năng lượng
  {
    path: 'ApprovedPowerProjects',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/approved-power-projects-page/approved-power-projects-page.module').then((m) => m.ApprovedPowerProjectsPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['375', '380', '381', '382', '383', '384']
    },
  },
  //Quản lý dự án nguồn điện đang đề xuất - Các dự án nguồn điện - Phòng quản lý năng lượng
  {
    path: 'ProposedPowerProjects',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/proposed-power-projects-page/proposed-power-projects-page.module').then((m) => m.ProposedPowerProjectsPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['375', '380', '385', '386', '387', '388']
    },
  },
  //Quản lý dự án điện mặt trời áp mái - Các dự án nguồn điện - Phòng quản lý năng lượng
  {
    path: 'RooftopSolarProjectManagement',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/rooftop-solar-project-management-page/rooftop-solar-project-management-page.module').then((m) => m.RooftopSolarProjectManagementPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['375', '380', '389', '390', '391', '392']
    },
  },
  //Quản lý công trình điện 110 KV và 220 KV trên tỉnh
  {
    path: 'ElectricalProjectManagement',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/electrical-project-management-page/electrical-project-management-page.module').then((m) => m.ElectricalProjectManagementPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['375', '397', '398', '399', '400']
    },
  },
  //Danh sách cơ sở sử dụng năng lượng trọng điểm
  {
    path: 'ListOfKeyEnergyUsers',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/list-of-key-energy-users-page/list-of-key-energy-users-page.module').then((m) => m.ListOfKeyEnergyUsersPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['375', '401', '402', '403', '404']
    },
  },
  // Quản lý thông tin quản lý về hoạt động điện lực
  {
    path: 'ManagementElectricityActivities',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/management-electricity-activities-page/management-electricity-activities-page.module').then((m) => m.ManagementElectricityActivitiesPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['375', '405', '406', '407', '408']
    },
  },
  // Các đơn vị hoạt động điện lực
  {
    path: 'ElectricOperatingUnits',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/electric-operating-units-page/electric-operating-units-page.module').then((m) => m.ElectricOperatingUnitsPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['375', '409', '410', '411', '412']
    },
  },
  /** -------------------------------------------------------------------------- Hết phòng quản lý năng lượng*/

  /**Danh mục Import --------------------------------------------------------------------------------------- */
  {
    path: 'import',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/category-import-page/category-import-page.module').then((m) => m.CategoryImportPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['488', '301', '302', '303', '304']
    },
  },
  /** ----------------------------------------------------------------------------------- Hết danh mục Import*/

  /**Nhật ký hệ thông -------------------------------------------------------------------------------------- */
  {
    path: 'logs',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/sys-logs-page/sys-logs-page.module').then((m) => m.SysLogsPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['488', '467', '468', '469', '470']
    },
  },
  /** ---------------------------------------------------------------------------------- Hết nhật ký hệ thông*/

  /**Cấu hình hệ thống ------------------------------------------------------------------------------------- */
  {
    path: 'Configs',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/config-page/config-page.module').then((m) => m.GroupConfigPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['488', '489', '490', '491', '492']
    },
  },
  /** ------------------------------------------------------------------------------------- Cấu hình hệ thống*/

  /**Quản lý tiêu chí nông thôn mới, nông thôn mới nâng cao ------------------------------------------------ */
  //Tiêu chí số 4 - Quản lý điện cấp xã
  {
    path: 'CommuneElectricityManagement',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/commune-electricity-management-page/electricity-management-page.module').then((m) => m.CommuneElectricityManagementPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['498', '393', '394', '395', '396']
    },
  },
  //Tiêu chí 7 - Tiêu chí cơ sở hạ tầng thương mại nông thôn
  {
    path: 'Target7',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/target7-page/target7-page.module').then((m) => m.Target7PageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['498', '137', '138', '139', '140']
    },
  },
  //Tiêu chí 17.08 - An toàn thực phẩm nông thôn mới
  {
    path: 'Target1708',
    loadChildren: () =>
      import('..//modules/app-pages/target1708-page/target1708-page.module').then((m) => m.Target1708PageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['498', '499', '500', '501', '502']
    },
  },
  //Quản lý tiêu chí nông thôn mới, nông thôn mới nâng cao
  {
    path: 'NewRuralCriteria',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/new-rural-criteria-page/new-rural-criteria-page.module').then((m) => m.NewRuralCriteriaPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['498', '503', '504', '505', '506']
    },
  },
  // Chỉ tiêu quản lý công nghiệp
  {
    path: 'IndustrialManagementTarget',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/industrial-management-target-page/target-page.module').then((m) => m.IndustrialManagementTargetPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['11', '515', '516', '517', '518']
    },
  },
  // Quản lý hội nghị, hội thảo
  {
    path: 'ManagementSeminar',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/management-seminar-page/management-seminar-page.module').then((m) => m.ManagementSeminarPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['220', '233', '507', '508', '509', '510']
    },
  },
  // Cơ sở bán hàng đa cấp
  {
    path: 'BusinessMultiLevel',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/business-multi-level-page/business-multi-level-page.module').then((m) => m.BusinessMultiLevelPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['220', '233', '511', '512', '513', '514']
    },
  },
  //Quản lý chợ do doanh nghiêp/ HTX đầu tư xây dựng, khai thác và quản lý
  {
    path: 'MarketInvestEnterprise',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/market-invest-enterprise-page/market-invest-enterprise-page.module').then((m) => m.MarketInvestEnterprisePageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '82', '519', '520', '521', '522']
    },
  },
  //Thông tin quy hoạch chợ
  {
    path: 'MarketPlanInformation',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/market-plan-information-page/market-plan-information-page.module').then((m) => m.MarketPlanInformationPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '82', '523', '524', '525', '526']
    },
  },
  //Thông tin quy hoạch, phát triển chợ
  {
    path: 'MarketDevelopPlan',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/market-develop-plan-page/market-develop-plan-page.module').then((m) => m.MarketDevelopPlanPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '82', '527', '528', '529', '530']
    },
  },
  //Quản lý chợ đạt tiêu chí số 7
  {
    path: 'MarketTargetSeven',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/market-target-seven-page/market-target-seven-page.module').then((m) => m.MarketTargetSevenPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['17', '82', '531', '532', '533', '534']
    },
  },
  /** ------------------------------------------------ Quản lý tiêu chí nông thôn mới, nông thôn mới nâng cao*/

  /**Khác -------------------------------------------------------------------------------------------------- */
  {
    path: 'demo-page',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/test-page/test-page.module').then((m) => m.TestPageModule),
    data: {
      layout: 'light-sct-sidebar'
    },
  },
  {
    path: 'test',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/custom-page/custom-page.module').then((m) => m.CustomPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['1', '2', '3']
    },
  },
  {
    path: 'controls',
    canActivate: [Permission],
    loadChildren: () =>
      import('..//modules/app-pages/controls-page/controls-page.module').then((m) => m.CustomPageModule),
    data: {
      layout: 'light-sct-sidebar',
      roles: ['1', '2', '3']
    },
  },
  {
    path: 'overview',
    loadChildren: () =>
      import('..//modules/account/account.module').then((m) => m.AccountModule),
    data: {
      layout: 'light-sct-sidebar'
    },
  },
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full',
  },
  {
    path: '**',
    redirectTo: 'error/404',
  },
  /** --------------------------------------------------------------------------------------------------- Hết*/

  /** Comment -------------------------------------------------------------------------------------------------
  {
    path: 'builder',
    loadChildren: () =>
      import('./builder/builder.module').then((m) => m.BuilderModule),
  },
  {
    path: 'crafted/pages/profile',
    loadChildren: () =>
      import('../modules/profile/profile.module').then((m) => m.ProfileModule),
    data: { layout: 'light-sct-sidebar' },
  },
  {
    path: 'crafted/account',
    loadChildren: () =>
      import('../modules/account/account.module').then((m) => m.AccountModule),
    data: { layout: 'dark-header' },
  },
  {
    path: 'crafted/pages/wizards',
    loadChildren: () =>
      import('../modules/wizards/wizards.module').then((m) => m.WizardsModule),
    data: { layout: 'light-header' },
  },
  {
    path: 'crafted/widgets',
    loadChildren: () =>
      import('../modules/widgets-examples/widgets-examples.module').then(
        (m) => m.WidgetsExamplesModule
      ),
    data: { layout: 'light-header' },
  },
  {
    path: 'apps/chat',
    loadChildren: () =>
      import('../modules/apps/chat/chat.module').then((m) => m.ChatModule),
    data: { layout: 'light-sct-sidebar' },
  },
  ['1', '2', '3', '4']
  --------------------------------------------------------------------------------------------------------------*/
];

export { Routing };
