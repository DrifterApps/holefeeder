import { Component, OnInit } from '@angular/core';
import { Account } from '@app/core/models';
import { AccountsService } from '@app/core/services';
import { filterNullish } from '@app/shared/helpers';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-account-upcoming',
  templateUrl: './account-upcoming.component.html',
  styleUrls: ['./account-upcoming.component.scss'],
})
export class AccountUpcomingComponent implements OnInit {
  account$!: Observable<Account>;

  constructor(private accountsService: AccountsService) {}

  ngOnInit() {
    this.account$ = this.accountsService.selectedAccount$.pipe(filterNullish());
  }
}
