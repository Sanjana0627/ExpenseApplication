import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavHeader } from "./components/nav-header/nav-header";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavHeader],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
// root component - just hosts the nav header and whichever page the router picks
export class App {
  protected readonly title = signal('expense-application-client');
}
