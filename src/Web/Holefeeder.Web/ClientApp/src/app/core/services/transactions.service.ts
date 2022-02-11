import {Injectable} from "@angular/core";
import {MessageService} from "@app/core/services/message.service";
import {Observable, tap} from "rxjs";
import {MessageAction} from "@app/shared/enums/message-action.enum";
import {MessageType} from "@app/shared/enums/message-type.enum";
import {TransactionsApiService} from "./api/transactions-api.service";
import {TransactionDetail} from "../models/transaction-detail.model";
import {PayCashflowCommand} from "../models/pay-cashflow-command.model";
import {MakePurchaseCommand} from "../models/make-purchase-command.model";
import {ModifyTransactionCommand} from "../models/modify-transaction-command.model";
import {PagingInfo} from "@app/core/models/paging-info.model";
import {TransferMoneyCommand} from "@app/core/models/transfer-money-command.model";

@Injectable({providedIn: 'root'})
export class TransactionsService {

  constructor(private apiService: TransactionsApiService, private messages: MessageService) {
  }


  find(accountId: string, offset: number, limit: number, sort: string[]): Observable<PagingInfo<TransactionDetail>> {
    return this.apiService.find(accountId, offset, limit, sort);
  }

  findById(id: string): Observable<TransactionDetail> {
    return this.apiService.findOneById(id);
  }

  payCashflow(transaction: PayCashflowCommand): Observable<string> {
    return this.apiService.payCashflow(transaction)
      .pipe(tap(id => this.messages.sendMessage(MessageType.transaction, MessageAction.post, {id: id})));
  }

  makePurchase(transaction: MakePurchaseCommand): Observable<string> {
    return this.apiService.makePurchase(transaction)
      .pipe(tap(id => this.messages.sendMessage(MessageType.transaction, MessageAction.post, {id: id})));
  }

  transfer(transaction: TransferMoneyCommand): Observable<string> {
    return this.apiService.transferMoney(transaction)
      .pipe(tap(id => this.messages.sendMessage(MessageType.transaction, MessageAction.post, {id: id})));
  }

  modify(transaction: ModifyTransactionCommand): Observable<void> {
    return this.apiService.modify(transaction)
      .pipe(tap(_ => this.messages.sendMessage(MessageType.transaction, MessageAction.post, {id: transaction.id})));
  }

  delete(id: string): Observable<void> {
    return this.apiService.delete(id)
      .pipe(tap(_ => this.messages.sendMessage(MessageType.transaction, MessageAction.delete, {id: id})));
  }
}
