import { ChangeDetectorRef, Component, ViewChild, OnInit } from '@angular/core';
import { ModalConfig, ModalComponent } from '../../_metronic/partials';
import { DashboardService } from './_services/dashboard.service';
import { Options } from 'select2';
import { ChartOptions } from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';
import * as moment from 'moment';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  @ViewChild('modal') private modalComponent: ModalComponent;
  modalConfig: ModalConfig = {
    modalTitle: 'Modal title',
    dismissButtonLabel: 'Submit',
    closeButtonLabel: 'Cancel'
  };

  isLoading$: any;
  colors: any;
  public formGroup: FormGroup;
  options: Options;

  fundingData: any;
  alcoholProductData: any;
  administrativeProceduresData: any;
  districtData: any;
  businessChartData: any;
  businessNameData: any;
  exportGoodsData: any;
  importGoodsData: any;
  type: string = 'Chợ'

  categoryData: any = [
    {
      id: 0,
      text: 'Quản lý chợ'
    },
    {
      id: 1,
      text: 'Quản lý kinh doanh xăng dầu'
    },
    {
      id: 2,
      text: 'Quản lý bán buôn thuốc lá'
    },
    {
      id: 3,
      text: 'Quản lý bán buôn rượu'
    }
  ]

  loginData = [
    {
      name: 'Lê Thị Bảo Vy',
      status: 1,
      time: '15'
    },
    {
      name: 'Phạm Văn Thọ',
      status: 0,
      time: '1'
    },
    {
      name: 'Nguyễn Thị Thu Hà',
      status: 1,
      time: '15'
    },
    {
      name: 'Hồ Võ Nhật Nguyên',
      status: 1,
      time: '120'
    },
    {
      name: 'Lê Thanh Phát',
      status: 1,
      time: '15'
    },
    {
      name: 'Nguyễn Nhật Trường',
      status: 0,
      time: '75'
    },
    {
      name: 'Lê Ngọc Sinh',
      status: 1,
      time: '135'
    },
    {
      name: 'Vũ Đình Cường',
      status: 1,
      time: '15'
    },
    {
      name: 'Phan Minh Nhân',
      status: 0,
      time: '75'
    },
    {
      name: 'Vũ Xuân Quyền',
      status: 1,
      time: '135'
    },
  ];

  categoryId = 0;
  yearRange: any = [];

  commerceChartData: any;
  MarketData: any = [];
  PetroleumData: any = [];
  CigaretteData: any = [];
  AlcoholData: any = [];

  TradePromotionProjectManagementData: any = [];

  DistrictDataStatus: Boolean = false;
  BusinessDataStatus: Boolean = false;
  MarketDataStatus: Boolean = false;
  AdministrativeProceduresDataStatus: Boolean = false;
  FundingDataStatus: Boolean = false;
  AlcoholProductDataStatus: Boolean = false;
  ImportExportDataStatus: Boolean = false;
  TradePromotionProjectManagementDataStatus: boolean = false;
  prepare_status: boolean = false;

  highLight: number = 0;
  empty: boolean = true;
  

  public getRandomColor() {
    var letters = '0123456789ABCDEF'.split('');
    var color = '#';
    for (var i = 0; i < 6; i++) {
      color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
  }

  public createYearRange() {
    for (var i = moment().year(); i > 1929; i--) {
      let obj = {
        id: i,
        text: i,
      }
      this.yearRange.push(obj);
    }
  }

  public doughnutChartOptions: ChartOptions<'doughnut'> = {
    responsive: false,
    plugins: {
      legend: {
        position: 'right'
      },
    },
    cutout: '75%',
    // rotation: 90,
  };

  public halfDoughnutChartOptions: ChartOptions<'doughnut'> = {
    responsive: false,
    plugins: {
      legend: {
        position: 'right',
      },
    },
    rotation: -90,
    circumference: 180,
    offset: 10,
    cutout: '60%',
  }

  public barChartXOptions: ChartOptions = {
    responsive: false,
    plugins: {
      legend: {
        display: false
      },
    },
    scales: {
      y: {
        ticks: {
          stepSize: 1
        },
        // title: {
        //   align: 'start',
        //   color: 'black',
        //   display: true,
        //   text: 'Doanh nghiệp',
        //   }
      },
      x: {
        // title: {
        //   align: 'start',
        //   color: 'black',
        //   display: true,
        //   text: 'Huyện'
        //   }
      }
    }
  };

  public barChartYOptions1: ChartOptions = {
    responsive: true,
    indexAxis: 'y',
    plugins: {
      legend: {
        display: false
      },
    },
  };

  public barChartYOptions: ChartOptions = {
    responsive: true,
    indexAxis: 'y',
    plugins: {
      legend: {
        display: false
      },
    },
    scales: {
      yAxes: {
        position: 'right',
        // display: false,
      },
      y: {
        display: false,
      }
    },
  };

  public barChartYReverseOptions: ChartOptions = {
    responsive: true,
    indexAxis: 'y',
    plugins: {
      legend: {
        display: false
      },
    },
    scales: {
      x: {
        reverse: true,
      },
    },
  };

  public pieChartOptions: ChartOptions<'pie'> = {
    responsive: false,
    plugins: {
      legend: {
        position: 'right'
      },
      datalabels: {
        formatter: function (value, context) {
          var sum = 0;
          context.dataset.data.forEach((i: any) => sum += i);
          return Math.round(value / sum * 100) + "%"
        },
        color: 'black',
      }
    },
    circumference: 360,
    // rotation: 90,
  }
  public pieChartPlugins = [ChartDataLabels];

  public lineChartOptions: ChartOptions = {
    responsive: false,
    plugins: {
      legend: {
        display: false
      },
    },
    borderColor: '#00FFFF',
  }

  public dualHorizontalBarChartOptions: ChartOptions = {
    responsive: true,
    indexAxis: 'y',
    elements: {
      bar: {
        borderWidth: 2,
      }
    },
    plugins: {
      legend: {
        position: 'top'
      },
    },
  }

  loginTrafficData = {
    labels: ['0h', '2h', '4h', '6h', '8h', '10h', '12h', '14h', '16h', '18h', '20h', '22h'],
    datasets: [
      {
        data: ['10', '32', '24', '6', '58', '110', '102', '14', '0', '18', '20', '12'],
        label: "Lưu lượng đăng nhập",
        fill: true,
        borderColor: "#04ADBF",
        backgroundColor: "rgba(104, 241, 255, 0.45)",
        pointBackgroundColor: '#04ADBF',
        pointBorderColor: '#04ADBF',
        // backgroundColor: [
        // 'rgba(54, 162, 235, 0.2)',
        // 'rgba(255, 206, 86, 0.2)',
        // 'rgba(75, 192, 192, 0.2)',
        // 'rgba(153, 102, 255, 0.2)',
        // 'rgba(255, 159, 64, 0.2)'
        // ],
        // borderColor: [
        // 'rgba(255,99,132,1)',
        // 'rgba(54, 162, 235, 1)',
        // 'rgba(255, 206, 86, 1)',
        // 'rgba(75, 192, 192, 1)',
        // 'rgba(153, 102, 255, 1)',
        // 'rgba(255, 159, 64, 1)'
        // ],
        // borderDash: [5, 5],
        // pointHoverBackgroundColor: "#55bae7",
        // pointHoverBorderColor: "#55bae7",
        // pointRadius: 0.5,
        // pointHoverRadius: 1
      }
    ],
  }

  constructor(
    public dashboardService: DashboardService,
    private fb: FormBuilder,
    private changeDetectorRef: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.isLoading$ = this.dashboardService.isLoading$;
    this.loadData();
    this.loadForm();
    this.options = {
      theme: 'bootstrap5',
      templateSelection: this.templateSelection,
    };
    this.createYearRange();
  }

  public templateSelection = (state: any): JQuery | string => {
    if (!state.id) {
      return state.text;
    }
    return jQuery('<span class="form-select form-select-solid form-select-lg">' + state.text + '</span>');
  }

  loadForm() {
    this.formGroup = this.fb.group({
      type: [0],
      business: [],
    })
    this.formGroup.controls.type.valueChanges.subscribe((x: number) => {
      if (x == 0) {
        this.type = 'Chợ';
        this.changeDetectorRef.detectChanges();
      } else {
        this.type = 'Doanh nghiệp';
        this.changeDetectorRef.detectChanges();
      }
    })
    this.formGroup.controls.business.valueChanges.subscribe((x: any) => {
      this.loadImportExportGoods(x);
    })
  }

  selectBusiness(index: number, id: any) {
    this.highLight = index;
    this.loadImportExportGoods(id);
  }

  count_online() {
    return 'Online: ' + this.loginData.filter((x: any) => x.status == 0).length.toString();
  }

  checkTime(value: any) {
    if (value <= 1)
      return 'Vừa mới'
    if (value < 60)
      return `${value} phút trước`
    else {
      const time = Math.floor(value / 60)
      return `${time} tiếng trước`
    }
  }

  // load data Phòng thương mại - Quản lý bán buôn thuốc lá
  loadCigaretteData() {
    let value: any[] = [];
    let district: any[] = [];
    this.dashboardService.loadCigarette().subscribe(res => {
      for (let itemDistrict of this.districtData) {
        let check = true;
        for (let item of res) {
          if (item.key == itemDistrict.id) {
            value.push(`${item.count}`)
            district.push(`${itemDistrict.name}`)
            check = false;
            break;
          }
        }
        if (check) {
          value.push(`0`)
          district.push(`${itemDistrict.name}`)
        }
      }
      const data = {
        labels: district,
        datasets: [
          {
            data: value,
            label: "Số lượng doanh nghiệp",
            backgroundColor: this.colors,
          }
        ],
      }
      this.CigaretteData = data;
    })
  }

  // load data Phòng thương mại - Quản lý bán buôn rượu
  loadAlcoholData() {
    let value: any[] = [];
    let district: any[] = [];
    this.dashboardService.loadAlcohol().subscribe(res => {
      for (let itemDistrict of this.districtData) {
        let check = true;
        for (let item of res) {
          if (item.key == itemDistrict.id) {
            value.push(`${item.count}`)
            district.push(`${itemDistrict.name}`)
            check = false;
            break;
          }
        }
        if (check) {
          value.push(`0`)
          district.push(`${itemDistrict.name}`)
        }
      }
      const data = {
        labels: district,
        datasets: [
          {
            data: value,
            label: "Số lượng doanh nghiệp",
            backgroundColor: this.colors,
          }
        ],
      }
      this.AlcoholData = data;
    })
  }

  // Phòng thương mại - Quản lý xăng dầu
  public loadPetroleumData() {
    let value: any[] = [];
    let district: any[] = [];
    this.dashboardService.loadDataPetroleum().subscribe(res => {
      for (let itemDistrict of this.districtData) {
        let check = true;
        for (let item of res) {
          if (item.key == itemDistrict.id) {
            value.push(`${item.count}`)
            district.push(`${itemDistrict.name}`)
            check = false;
            break;
          }
        }
        if (check) {
          value.push(`0`)
          district.push(`${itemDistrict.name}`)
        }
      }
      const data = {
        labels: district,
        datasets: [
          {
            data: value,
            label: "Số lượng doanh nghiệp",
            backgroundColor: this.colors,
          }
        ],
      }
      this.PetroleumData = data;
    })
  }

  // Phòng thương mại - Quản lý chợ
  public loadMarketData() {
    let value: any[] = [];
    let district: any[] = [];
    this.dashboardService.loadDataMarket().subscribe(res => {
      for (let itemDistrict of this.districtData) {
        let check = true;
        for (let item of res) {
          if (item.key == itemDistrict.id) {
            value.push(`${item.count}`)
            district.push(`${itemDistrict.name}`)
            check = false;
            break;
          }
        }
        if (check) {
          value.push(`0`)
          district.push(`${itemDistrict.name}`)
        }
      }
      const data = {
        labels: district,
        datasets: [
          {
            data: value,
            label: "Số lượng chợ",
            backgroundColor: this.colors,
          }
        ],
      }
      this.MarketData = data;
      this.commerceChartData = data;
      this.MarketDataStatus = true;
      this.check_status();
      this.changeDetectorRef.detectChanges();
    })
  }

  // load data huyện
  public loadDistrictData() {
    this.dashboardService.loadDistrict().subscribe(res => {
      let districtItem: any[] = [];
      for (let item of res.items) {
        districtItem.push({
          id: item.districtId,
          name: item.districtName
        })
      }
      this.districtData = districtItem;
      let colormap = require('colormap')
      let list_colors = colormap({
        colormap: 'summer',
        nshades: this.districtData.length,
        format: 'hex',
        alpha: 1
      })
      this.colors = list_colors
      this.loadDataBusiness();
      this.loadPetroleumData();
      this.loadCigaretteData();
      this.loadAlcoholData();
      this.loadMarketData();
    }
    )
  }

  //load data thủ tục hành chính
  public loadAdministrativeProceduresData() {
    this.dashboardService.loadDataStatus().subscribe(res => {
      let value: any[] = [];
      for (let item of res) {
        value.push(`${item.count}`);
      }
      const data = {
        labels: [`Chưa xử lý`, `Đang xử lý`, `Đã xử lý`],
        datasets: [
          {
            data: value,
            backgroundColor: [
              '#F25A38',
              '#F2B807',
              '#04ADBF',
            ],
            label: "Số lượng",
          }
        ],
      }
      this.administrativeProceduresData = data;
      this.AdministrativeProceduresDataStatus = true;
      this.check_status();
    })
  }

  //load data quản lý đề án xúc tiến thương mại 
  public loadTradePromotionProjectManagementData() {
    this.dashboardService.loadTradePromotionProjectManagement().subscribe(res => {
      let value: any[] = [res.count0, res.count1];
      const data = {
        labels: [`Không thực hiện`, `Có thực hiện`],
        datasets: [
          {
            data: value,
            backgroundColor: [
              '#F25A38',
              '#04ADBF',
            ],
            label: "Số lượng",
          }
        ],
      }
      this.TradePromotionProjectManagementData = data;
      this.TradePromotionProjectManagementDataStatus = true;
      this.check_status();
    })
  }

  //load data kinh phí khuyến nông
  public loadFundingData() {
    this.dashboardService.loadDataFunding().subscribe(res => {
      const value = [res.sumIndustrialPromotionFunding, res.sumReciprocalEnterpriseFunding]
      const data = {
        labels: ["Kinh phí khuyến công hỗ trợ", "Kinh phí doanh nghiệp đối ứng"],
        datasets: [
          {
            data: value,
            label: "Kinh phí",
            backgroundColor: [
              '#04ADBF',
              '#F25A38',
            ],
          }
        ],
      }
      this.fundingData = data;
      this.FundingDataStatus = true;
      this.check_status();
    })
  }

  //load data tình hình bán lẻ rượu
  public loadDataAlcoholProduct() {
    this.dashboardService.loadDataAlcoholProduct().subscribe(res => {
      const value = [res.craftAncol, res.industryAncol]
      const data = {
        labels: ["Thủ công", "Công nghiệp"],
        datasets: [
          {
            data: value,
            label: "Tình hình bán lẻ rượu",
            backgroundColor: [
              '#04ADBF',
              '#F25A38',
            ],
          }
        ],
      }
      this.alcoholProductData = data;
      this.AlcoholProductDataStatus = true;
      this.check_status()
    })
  }

  //load data số lượng doanh nghiệp
  public loadDataBusiness() {
    this.dashboardService.loadDataBusiness().subscribe(res => {
      let dataBusinessGet: any[] = [];
      let dataDistrictGet: any[] = [];
      for (let itemDistrict of this.districtData) {
        let check = true;
        for (let item of res) {
          if (item.key == itemDistrict.id) {
            dataBusinessGet.push(`${item.count}`)
            dataDistrictGet.push(`${itemDistrict.name}`)
            check = false;
            break;
          }
        }
        if (check) {
          dataBusinessGet.push(`0`)
          dataDistrictGet.push(`${itemDistrict.name}`)
        }
      }
      const data = {
        labels: dataDistrictGet,
        datasets: [
          {
            data: dataBusinessGet,
            label: "Số lượng doanh nghiệp",
            backgroundColor: this.colors,
          }
        ],
      }
      this.businessChartData = data;
      this.BusinessDataStatus = true;
      this.check_status();
      this.changeDetectorRef.detectChanges();
    })
  }

  //load data tên doanh nghiệp
  public loadBusinessNameData() {
    this.dashboardService.loadBusinessName().subscribe(res => {
      const listItem: any[] = [];
      for (let item of res) {
        let obj = {
          id: item.businessId,
          text: item.businessName,
        }
        listItem.push(obj);
      }
      this.businessNameData = listItem;
      this.loadImportExportGoods(listItem[0].id)
    })
  }

  //load data xuất nhập khẩu
  public loadImportExportGoods(id: any) {
    this.dashboardService.loadExImpordDataById(id).subscribe(res => {
      let emptyData = true;
      if (res.volumeExport.length != 0 || res.volumeImport.length != 0) {
        emptyData = false;
      }
      this.empty = emptyData;

      const listItem: any[] = [];
      for (let item of res.volumeExport) {
        if (!listItem.includes(item.exportGoodsName)) {
          listItem.push(item.exportGoodsName);
        }
      }
      for (let item of res.volumeImport) {
        if (!listItem.includes(item.importGoodsName)) {
          listItem.push(item.importGoodsName);
        }
      }

      const exportData: any[] = [];
      const importData: any[] = [];
      for (let item of listItem) {
        let check = true;
        for (let itemEx of res.volumeExport) {
          if (item == itemEx.exportGoodsName) {
            exportData.push(`${itemEx.amount}`)
            check = false;
            break;
          }
        }
        if (check) {
          exportData.push('0');
        }
      }

      for (let item of listItem) {
        let check = true;
        for (let itemIm of res.volumeImport) {
          if (item == itemIm.importGoodsName) {
            importData.push(`${itemIm.amount}`)
            check = false;
            break;
          }
        }
        if (check) {
          importData.push('0');
        }
      }

      const data_im = {
        labels: listItem,
        datasets: [
          {
            data: importData,
            label: "Nhập khẩu",
            backgroundColor: [
              '#04ADBF',
            ],
          },
        ],
      }
      this.importGoodsData = data_im;

      const data_ex = {
        labels: listItem,
        datasets: [
          {
            data: exportData,
            label: "Xuất khẩu",
            backgroundColor: [
              '#F25A38',
            ],
            yAxesID: "y-axis-right",
          },
        ],
      }

      this.exportGoodsData = data_ex;
      this.ImportExportDataStatus = true;
      this.check_status();
      this.changeDetectorRef.detectChanges();
    })
  }

  public loadData() {
    this.loadDistrictData();
    this.loadBusinessNameData();
    this.loadAdministrativeProceduresData();
    this.loadFundingData();
    this.loadDataAlcoholProduct();
    this.loadTradePromotionProjectManagementData();
  }

  public check_status() {
    if (this.BusinessDataStatus
      == this.MarketDataStatus
      == this.AdministrativeProceduresDataStatus
      == this.FundingDataStatus
      == this.AlcoholProductDataStatus
      == this.ImportExportDataStatus
      == this.TradePromotionProjectManagementDataStatus
      && this.ImportExportDataStatus == true) {
      this.prepare_status = true;
    }
  }

  async openModal() {
    return await this.modalComponent.open();
  }
}
