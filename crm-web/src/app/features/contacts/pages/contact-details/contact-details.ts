import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ContactService } from '../../services/contact.service';
import { TimelineService } from '../../../activities/services/timeline.service';
import { Timeline } from '../../../activities/components/timeline';
import { Contact } from '../../../../core/models/entities.model';
import { TimelineGroup } from '../../../activities/timeline.model';

@Component({
  selector: 'app-contact-details',
  imports: [RouterLink, Timeline],
  templateUrl: './contact-details.html',
  styleUrl: './contact-details.scss'
})
export class ContactDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly contacts = inject(ContactService);
  private readonly timeline = inject(TimelineService);

  readonly contact = signal<Contact | null>(null);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  // One backend call now returns the merged, sorted, grouped timeline.
  readonly groups = signal<TimelineGroup[]>([]);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) { this.error.set('No contact id.'); this.loading.set(false); return; }

    this.contacts.getById(id).subscribe({
      next: (c) => { this.contact.set(c); this.loading.set(false); },
      error: () => { this.error.set('Contact not found.'); this.loading.set(false); }
    });

    this.timeline.forContact(id).subscribe({
      next: (g) => this.groups.set(g),
      error: () => { /* timeline degrades gracefully */ }
    });
  }
}
