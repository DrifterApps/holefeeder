import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { Location } from '@angular/common';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CashflowsService } from '@app/shared/services/cashflows.service';
import { NgbDateAdapter, NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Transaction } from '@app/shared/models/transaction.model';
import { AccountsService } from '@app/shared/services/accounts.service';
import { ICategory } from '@app/shared/interfaces/category.interface';
import { CategoriesService } from '@app/shared/services/categories.service';
import { TransactionsService } from '@app/shared/services/transactions.service';
import { IAccount } from '@app/shared/interfaces/account.interface';
import { NgbDateParserAdapter } from '@app/shared/ngb-date-parser.adapter';
import { startOfToday } from 'date-fns';
import { faCalendarAlt } from '@fortawesome/free-solid-svg-icons';
import { ITransactionDetail } from '@app/shared/interfaces/transaction-detail.interface';
import { TransactionDetail } from '@app/shared/models/transaction-detail.model';
import { IPagingInfo } from '@app/shared/interfaces/paging-info.interface';
import { MakePurchaseCommand } from '@app/shared/transactions/make-purchase-command.model';
import { PayCashflowCommand } from '@app/shared/transactions/pay-cashflow-command.model';
import { ICashflowDetail } from '@app/shared/interfaces/cashflow-detail.interface';
import { ModifyTransactionCommand } from '@app/shared/transactions/modify-transaction-command.model';

@Component({
  selector: 'dfta-transaction-edit',
  templateUrl: './transaction-edit.component.html',
  styleUrls: ['./transaction-edit.component.scss'],
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateParserAdapter }]
})
export class TransactionEditComponent implements OnInit {
  @ViewChild('confirm', { static: true })
  confirmModalElement: ElementRef;
  confirmModal: NgbModalRef;
  confirmMessages: string;

  account: string;
  transaction: ITransactionDetail;
  transactionForm: FormGroup;

  accounts: IPagingInfo<IAccount>;
  categories: ICategory[];
  cashflow: ICashflowDetail;
  cashflowDate: Date;

  isLoaded = false;
  isNew = false;
  payCashflow = false;

  faCalendarAlt = faCalendarAlt;

  constructor(
    private route: ActivatedRoute,
    private accountsService: AccountsService,
    private cashflowsService: CashflowsService,
    private categoriesService: CategoriesService,
    private transactionsService: TransactionsService,
    private modalService: NgbModal,
    private formBuilder: FormBuilder,
    private location: Location
  ) {
  }

  async ngOnInit() {
    this.accounts = await this.accountsService.find(null, null, [
      '-favorite',
      'name'
    ], [
      'inactive:eq:false'
    ]);

    this.payCashflow = this.route.snapshot.queryParamMap.has('cashflow');

    this.transactionForm = this.formBuilder.group({
      amount: ['', [Validators.required, Validators.min(0)]],
      date: [''],
      account: [{value: '', disabled: this.payCashflow}, [Validators.required]],
      category: [{value: '', disabled: this.payCashflow}, [Validators.required]],
      description: [{value: '', disabled: this.payCashflow}],
      tags: this.formBuilder.array([])
    });

    this.categories = await this.categoriesService.find();

    this.account = this.route.parent.snapshot.queryParamMap.get('accountId');

    if (this.route.snapshot.paramMap.has('transactionId')) {
      const transactionId = this.route.snapshot.paramMap.get('transactionId');
      this.transaction = await this.transactionsService.findOneById(transactionId);
    } else {
      this.isNew = true;
      const date = this.route.snapshot.queryParamMap.has('date') ? new Date(this.route.snapshot.queryParamMap.get('date')) : startOfToday();
      if (this.payCashflow) {
        const cashflowId = this.route.snapshot.queryParamMap.get('cashflow');
        this.cashflow = await this.cashflowsService.findOneById(cashflowId);
        this.cashflowDate = date;

        this.transaction = Object.assign(new TransactionDetail(), {
          account: this.cashflow.account,
          cashflow: this.cashflow.id,
          category: this.cashflow.category,
          amount: this.cashflow.amount,
          id: undefined,
          date: date,
          cashflowDate: date,
          description: this.cashflow.description,
          tags: this.cashflow.tags ? this.cashflow.tags : []
        });
      } else {
        this.transaction = Object.assign(new TransactionDetail(), {
          account: this.account ? this.account : this.accounts[0].id,
          date: date,
          category: this.categories[0].id
        });
      }
    }

    this.transactionForm.patchValue({
      amount: this.transaction.amount,
      date: this.transaction.date,
      account: this.transaction.account.id,
      category: this.transaction.category.id,
      description: this.transaction.description
    });
    if (this.transaction.tags) {
      const tags = this.transactionForm.get('tags') as FormArray;
      this.transaction.tags.forEach(t => tags.push(this.formBuilder.control(t)));
    }
    this.isLoaded = true;
  }

  goBack(): void {
    this.location.back();
  }

  async onSubmit() {
    const transaction = Object.assign(
      new Transaction(),
      this.transaction,
      this.transactionForm.value
    );
    if (this.transaction.id) {
      await this.transactionsService.modify(
        new ModifyTransactionCommand(Object.assign({}, this.transactionForm.value,{
          id: this.transaction.id,
          accountId: this.transactionForm.value.account,
          categoryId: this.transactionForm.value.category
        })));
    } else {
      if(this.payCashflow){
        await this.transactionsService.payCashflow(
          new PayCashflowCommand(Object.assign({}, this.transactionForm.value,{
            cashflowId: this.cashflow.id,
            cashflowDate: this.cashflowDate
          })));
      } else {
        await this.transactionsService.makePurchase(
          new MakePurchaseCommand(Object.assign({}, this.transactionForm.value,{
            accountId: this.transactionForm.value.account,
            categoryId: this.transactionForm.value.category
          })));
      }
    }

    this.location.back();
  }

  onDelete(content: any) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-delete-title' }).result.then(async (_) => {
      await this.transactionsService.delete(this.transaction.id);
      this.location.back();
    }, (_) => { });
  }

  onConfirm() { }

  compareFn(optionOne: any, optionTwo: any): boolean {
    if (optionOne && optionTwo) {
      const idOne = typeof optionOne === 'string' ? optionOne : optionOne.id;
      const idTwo = typeof optionTwo === 'string' ? optionTwo : optionTwo.id;
      return idOne === idTwo;
    }
    return false;
  }
}
