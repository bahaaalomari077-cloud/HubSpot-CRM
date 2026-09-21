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
    { path: '/pipeline', label: 'Pipeline' },
    { path: '/dashboard', label: 'Dashboard' },
    { path: '/contacts', label: 'Contacts' },
    { path: '/companies', label: 'Companies' },
    { path: '/applications', label: 'Applications' }
  ];
}
