export class ModifyCashflowCommand {
  constructor(
    public id: string,
    public amount: number,
    public effectiveDate: Date,
    public description: string,
    public tags: string[]
  ) {}
}
