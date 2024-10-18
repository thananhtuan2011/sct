import { AuthService } from 'src/app/modules/auth';
import { Injectable, Inject, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { TableService } from 'src/app/_metronic/shared/crud-table/services/table.service';
import { environment } from 'src/environments/environment';

@Injectable({
	providedIn: 'root'
})
export class CommonService {
	API_URL = `${environment.apiUrl}/Common`;

	constructor(private http: HttpClient,
		private _AuthService: AuthService) {
	}

	getListPostion(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-chuc-vu', { headers: httpHeaders });
	}

	getListDepartmemt(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-don-vi', { headers: httpHeaders });
	}

	getUser(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-user', { headers: httpHeaders });
	}

	getBusiness(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-doanh-nghiep', { headers: httpHeaders });
	}

	getCommune(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-xa', { headers: httpHeaders });
	}

	getListCountry(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-country', { headers: httpHeaders });
	}

	getTypeOfMarket(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-typeofmarket', { headers: httpHeaders });
	}

	getTypeOfEnergy(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-typeofenergy', { headers: httpHeaders });
	}

	getEnergyIndustry(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-energyindustry', { headers: httpHeaders });
	}

	getTypeOfTradePromotion(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-typeoftradepromotion', { headers: httpHeaders });
	}

	getTradePromotionProject(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-tradepromotionproject', { headers: httpHeaders });
	}

	getTypeOfBusiness(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-typeofbusiness', { headers: httpHeaders });
	}

	getTypeOfProfession(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-typeofprofession', { headers: httpHeaders });
	}

	getIndustry(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-industry', { headers: httpHeaders });
	}

	getDistrict(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-district', { headers: httpHeaders });
	}

	getStateUnit(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-stateunits', { headers: httpHeaders });
	}

	getStateTitle(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-statetitles', { headers: httpHeaders });
	}

	getAdministrativeFormality(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-adminformalities', { headers: httpHeaders });
	}

	getCriteria(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-cri', { headers: httpHeaders });
	}

	getBusinessLine(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-businessline', { headers: httpHeaders });
	}

	getCommercial(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/list-commercial', { headers: httpHeaders });
	}

	GetConfig(Code: string): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.post<any>(`${this.API_URL}/GetConfig`, `"${Code}"`, { headers: httpHeaders });
	}
	
	getListVoltageLevel(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/ListVoltageLevel', { headers: httpHeaders });
	}
	
	getListTypeOfConstruction(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/ListTypeOfConstruction', { headers: httpHeaders });
	}
	
	getListInstallCapacity() : any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/ListInstallCapacity', { headers: httpHeaders });
	}
	
	getListCertificateType() : any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/ListCertificateType', { headers: httpHeaders });
	}
	
	getListContractForm() : any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/ListContractForm', { headers: httpHeaders });
	}
	
	//Báo cáo tình hình hoạt động của dự án đầu tư trong cụm công nghiệp
	getListTargetCC1(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/ListTargetCCN1', { headers: httpHeaders });
	}
	
	getListTargetCCN2(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/ListTargetCCN2', { headers: httpHeaders });
	}
	
	getListTargetCCN3(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/ListTargetCCN3', { headers: httpHeaders });
	}

	loadStage(): any {
		const httpHeaders = this._AuthService.getHTTPHeaders();
		return this.http.get<any>(this.API_URL + '/LoadStage', { headers: httpHeaders });
	}
}
