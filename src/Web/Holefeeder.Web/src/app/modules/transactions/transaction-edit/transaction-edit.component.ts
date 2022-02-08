import { Component, OnInit } from '@angular/core';
import { FormGroup, FormArray, FormGroupDirective } from '@angular/forms';
import { NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateParserAdapter } from '@app/shared/ngb-date-parser.adapter';
import { combineLatest, forkJoin, Observable, tap } from 'rxjs';
import { CategoriesService } from '@app/core/services/categories.service';
import { Category } from '@app/core/models/category.model';
import { AccountInfo } from '@app/core/models/account-info.model';
import { AccountsInfoService } from '@app/core/services/account-info.service';

@Component({
  selector: 'dfta-transaction-edit',
  templateUrl: './transaction-edit.component.html',
  styleUrls: ['./transaction-edit.component.scss'],
  providers: [{ provide: NgbDateAdapter, useClass: NgbDateParserAdapter }]
})
export class TransactionEditComponent implements OnInit {

  form: FormGroup;

  values$: Observable<{ accounts: AccountInfo[], categories: Category[] }>;

  constructor(
    private rootFormGroup: FormGroupDirective,
    private accountsService: AccountsInfoService,
    private categoriesService: CategoriesService
  ) {
  }

  ngOnInit() {
    this.form = this.rootFormGroup.control;

    this.values$ = combineLatest({
      accounts: this.accountsService.accounts$,
      categories: this.categoriesService.categories$
    });
  }

  get tags(): FormArray { return this.form.get('tags') as FormArray }
}
