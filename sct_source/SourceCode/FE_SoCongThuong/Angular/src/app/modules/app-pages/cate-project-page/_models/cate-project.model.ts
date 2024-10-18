import { BaseModel } from '../../../../_metronic/shared/crud-table/models/base.model';

export interface CateProjectModel extends BaseModel {
    // petroleumBusinessId : string;
    cateProjectId: string;
    units: string;
    country: string;
    companySell: string;
    address: string;

    projectType: number;
    projectTypeName: string;
    area: number;
    investors: string;
    investmentCertificateCode: string;
    policyDecisions: string;

    projectId: string;
    projectName: string;
    projectInvestment: number | null;
    projectInvestmentUnits: string;

    /// Địa điểm thực hiện dự án
    projectAddress: string;

    /// ngành nghề
    profession: string;

    /// người đại diện pháp luật
    projectLegalRepresent: string;

    /// số diện thoại liên lạc
    projectPhoneNumber: string;

    /// tiến độ thực hiện dự án, tiến độ đã đăng ký
    projectProgress: string;

    /// thời gian thực hiện (năm)
    projectOperatingTime: number;

    /// tiến độ thực tế
    projectProgressActual: string;

    /// địa bàn
    projectLocalArea: string;

    /// quốc tịch/đối tác
    projectPartnerNationality: string;

    /// hình thức đầu tư
    projectInvestmentForm: string;

    /// năm cấp phép
    projectLicenseYear: number;

    /// Năm thực hiện
    projectImplementationYear: number;

    /// mục tiêu, quy mô thực hiện dự án
    projectImplementationScale: string;

    /// quyết định thu hồi
    projectDecisionToWithdraw: string;

    /// FDI
    projectFdi: string;
    note: string;

    /// công ty bán
    companyBuy: string;

    /// vốn điều lệ ban đầu
    initialCharterCapital: number;

    /// vốn mua
    capitalPurchase: number;

    /// mua thực tế
    actualPurchase: number;

    /// vốn điều lệ sau khi mua
    charterCapitalAfterPurchase: number;

    investmentCertificateDate:any, policyDecisionsDate:any, projectDecisionToWithdrawDate:any, capitalContributionTradingTime:any, details: any[];
    historys: any[];

}
export interface CateProjectDisbursementModel extends BaseModel {
    // CigaretteBusinessId : string;
    CateProjectId: string;
    disbursementDate:any, disbursementMoney: number;
    disbursementUnits: string;
    isConfirm: boolean;

}
