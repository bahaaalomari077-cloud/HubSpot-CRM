import { Component, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { TimelineGroup, TimelineItem } from '../timeline.model';
import { ActivityService, ACTIVITY_TYPE } from '../services/activity.service';

/** Maps a timeline item's subType/kind to an icon key + accent color. */
interface IconSpec { key: string; color: string; }

@Component({
  selector: 'app-timeline',
  imports: [DatePipe, FormsModule],
  templateUrl: './timeline.html',
  styleUrl: './timeline.scss'
})
export class Timeline {
  private readonly activities = inject(ActivityService);

  /** Date-grouped, newest-first data straight from the backend timeline endpoint. */
  readonly groups = input.required<TimelineGroup[]>();

  /** Raised after an edit or delete so the parent can refetch. */
  readonly changed = output<void>();

  /** Id of the activity currently being edited inline. */
  readonly editingId = signal<string | null>(null);
  editText = '';
  readonly busy = signal(false);

  /** Only activities are editable — a message is a record of what was sent. */
  canEdit(item: TimelineItem): boolean {
    return item.kind === 'Activity';
  }

  startEdit(item: TimelineItem): void {
    this.editText = item.detail ?? '';
    this.editingId.set(item.id);
  }

  cancelEdit(): void {
    this.editingId.set(null);
    this.editText = '';
  }

  saveEdit(item: TimelineItem): void {
    const text = this.editText.trim();
    if (this.busy()) return;

    this.busy.set(true);
    this.activities.edit(item.id, {
      type: this.typeIdFor(item.subType),
      subject: item.title,
      description: text,
      status: item.isCompleted ? 'Completed' : 'Pending',
      isCompleted: !!item.isCompleted
    }).subscribe({
      next: () => { this.busy.set(false); this.cancelEdit(); this.changed.emit(); },
      error: () => { this.busy.set(false); }
    });
  }

  removeItem(item: TimelineItem): void {
    if (!confirm(`Delete "${item.title}"?`)) return;
    this.busy.set(true);
    this.activities.remove(item.id).subscribe({
      next: () => { this.busy.set(false); this.changed.emit(); },
      error: () => { this.busy.set(false); }
    });
  }

  /** Map the timeline's subType label back to the backend enum id. */
  private typeIdFor(subType: string): number {
    switch (subType) {
      case 'Meeting': return ACTIVITY_TYPE.Meeting;
      case 'Call':    return ACTIVITY_TYPE.Call;
      case 'Task':    return ACTIVITY_TYPE.Task;
      case 'Email':   return ACTIVITY_TYPE.Email;
      default:        return ACTIVITY_TYPE.Note;
    }
  }

  /** Pick an icon + color for an item, keyed off its subType (falls back on kind). */
  iconFor(item: TimelineItem): IconSpec {
    switch (item.subType) {
      case 'Call':      return { key: 'call',     color: '#3b82f6' };
      case 'Meeting':   return { key: 'meeting',  color: '#8b5cf6' };
      case 'Task':      return { key: 'task',     color: '#10b981' };
      case 'Note':      return { key: 'note',     color: '#64748b' };
      case 'Email':     return { key: 'email',    color: '#f59e0b' };
      case 'WhatsApp':  return { key: 'whatsapp', color: '#10b981' };
      case 'Sms':       return { key: 'sms',      color: '#3b82f6' };
      case 'Telegram':  return { key: 'telegram', color: '#3b82f6' };
      case 'LiveChat':  return { key: 'chat',     color: '#8b5cf6' };
      default:
        return item.kind === 'Message'
          ? { key: 'chat', color: '#10b981' }
          : { key: 'task', color: '#3b82f6' };
    }
  }
}
