import { Injectable } from '@angular/core';
import { ApiService } from '@app/shared/services/api.service';
import { ICashflow, cashflowToServer, cashflowFromServer } from '../interfaces/cashflow.interface';
import { map } from 'rxjs/operators';
import { HttpParams } from '@angular/common/http';
import { cashflowDetailFromServer, ICashflowDetail } from '../interfaces/cashflow-detail.interface';
import { PagingInfo } from '../interfaces/paging-info.interface';

@Injectable()
export class CashflowsService {
  private basePath = 'cashflows';

  constructor(private api: ApiService) { }

  find(
    offset: number,
    limit: number,
    sort: string[],
    filter: string[]
  ): Promise<PagingInfo<ICashflowDetail>> {
    let params = new HttpParams();
    if (offset) {
      params = params.set('offset', `${offset}`);
    }
    if (limit) {
      params = params.set('limit', `${limit}`);
    }
    if (sort) {
      sort.forEach(element => {
        params = params.append('sort', `${element}`);
      });
    }
    if (filter) {
      filter.forEach(element => {
        params = params.append('filter', `${element}`);
      });
    }
    return this.api
      .find(`${this.api.budgetingBasePath}/${this.basePath}`, params)
      .pipe(
        map(data =>
          Object.assign(
            {},
            {
              totalCount: data.totalCount,
              items: Object.assign(
                [],
                data.items.map(cashflowDetailFromServer)
              )
            }
          )
        )
      )
      .toPromise();
  }

  findOneById(id: number | string): Promise<ICashflowDetail> {
    return this.api
      .get(`${this.api.budgetingBasePath}/${this.basePath}/${id}`)
      .pipe(
        map(cashflowDetailFromServer)
      )
      .toPromise();
  }

  create(cashflow: ICashflow): Promise<ICashflow> {
    return this.api
      .post(`${this.api.budgetingBasePath}/${this.basePath}`, cashflowToServer(cashflow))
      .pipe(
        map(cashflowFromServer)
      )
      .toPromise();
  }

  update(
    id: number | string,
    cashflow: ICashflow
  ): Promise<void> {
    return this.api
      .put(`${this.api.budgetingBasePath}/${this.basePath}/${id}`, cashflowToServer(cashflow))
      .toPromise();
  }
}
