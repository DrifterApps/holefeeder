import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AccountTypeNames } from '@app/shared/enums/account-type.enum';
import { NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateParserAdapter } from '@app/shared/ngb-date-parser.adapter';
import { AccountsService } from '@app/shared/services/accounts.service';
import { IAccount } from '@app/shared/interfaces/account.interface';
import { Account } from '@app/shared/models/account.model';
import { ModifyAccountCommand } from '@app/shared/accounts/modify-account-command.model';
import { OpenAccountCommand } from '@app/shared/accounts/open-account-command.model';
import { Observable, tap } from 'rxjs';

@Component({
  selector: 'dfta-account-edit',
  templateUrl: './account-edit.component.html',
  styleUrls: ['./account-edit.component.scss'],
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateParserAdapter }]
})
export class AccountEditComponent implements OnInit {
  account$: Observable<IAccount> | undefined;
  isDirty = true;

  accountForm: FormGroup;

  accountTypesNames = AccountTypeNames;

  constructor(
    private activatedRoute: ActivatedRoute,
    private accountsService: AccountsService,
    private formBuilder: FormBuilder,
    private router: Router
  ) {
    this.accountForm = this.formBuilder.group({
      name: [null, Validators.required],
      type: [null, Validators.required],
      openBalance: [0, Validators.required],
      openDate: [null, Validators.required],
      description: [],
      favorite: [],
      inactive: [],
    });
  }

  async ngOnInit() {
    this.activatedRoute.params.subscribe(async params => {
      this.account$ = this.accountsService.findOneByIdWithDetails(params['accountId'])
        .pipe(
          tap(account => this.accountForm.patchValue(account ?? new Account()))
        );
    });
  }

  async onSubmit(account: IAccount) {
    let accountId: string;
    if (account.id) {
      accountId = account.id;
      await this.accountsService.modify(
        new ModifyAccountCommand(Object.assign({}, this.accountForm.value, {
          id: account.id
        }))
      );
    } else {
      accountId = await this.accountsService.open(
        new OpenAccountCommand(Object.assign({}, this.accountForm.value))
      );
    }
    this.router.navigate(['accounts', accountId]);
  }

  compareFn(optionOne: any, optionTwo: any): boolean {
    if (optionOne && optionTwo) {
      const idOne = typeof optionOne === 'string' ? optionOne : optionOne.id;
      const idTwo = typeof optionTwo === 'string' ? optionTwo : optionTwo.id;
      return idOne === idTwo;
    }
    return false;
  }
}
