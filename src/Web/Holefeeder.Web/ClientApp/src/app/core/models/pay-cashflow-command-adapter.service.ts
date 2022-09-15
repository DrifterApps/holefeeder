import { Injectable } from "@angular/core";
import { Adapter } from "@app/shared";
import { dateToUtc } from "@app/shared/helpers";
import { PayCashflowCommand } from "@app/core/models/pay-cashflow-command.model";

@Injectable({ providedIn: "root" })
export class PayCashflowCommandAdapter implements Adapter<PayCashflowCommand> {
  adapt(item: any): PayCashflowCommand {
    return new PayCashflowCommand(
      dateToUtc(item.date),
      item.amount,
      item.cashflow,
      dateToUtc(item.cashflowDate)
    );
  }
}
