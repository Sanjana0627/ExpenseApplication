import { Pipe, PipeTransform } from '@angular/core';

const CURRENCY_SYMBOLS: Record<number, string> = {
  1: '₺',   
  2: '€',   
  3: '$',   
  4: '₨',   
  5: '₹',
  6: 'د.إ'    
};

@Pipe({
  name: 'appCurrency',
  standalone: true
})
// formats an amount with the right currency symbol 
export class AppCurrencyPipe implements PipeTransform {
  transform(amount: number, currencyId: number): string {
    const symbol = CURRENCY_SYMBOLS[currencyId] || '';
    const formatted = (amount ?? 0).toLocaleString('en-US', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    });
    return `${symbol}${formatted}`;
  }
}