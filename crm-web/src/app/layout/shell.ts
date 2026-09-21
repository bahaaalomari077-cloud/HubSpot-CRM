import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-shell',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './shell.html',
  styleUrl: './shell.scss'
})
export class Shell {
  readonly nav = [
    { path: '/dashboard', label: 'Dashboard' },
    { path: '/contacts', label: 'Contacts' },
    { path: '/applications', label: 'Applications' }
  ];
}
