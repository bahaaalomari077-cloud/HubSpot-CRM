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
    { path: '/pipeline', label: 'Pipeline', icon: 'board' },
    { path: '/dashboard', label: 'Dashboard', icon: 'grid' },
    { path: '/contacts', label: 'Contacts', icon: 'user' },
    { path: '/companies', label: 'Companies', icon: 'building' },
    { path: '/applications', label: 'Applications', icon: 'doc' }
  ];
}
