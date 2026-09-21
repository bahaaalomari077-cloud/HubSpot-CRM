import { Component, input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { TimelineGroup, TimelineItem } from '../timeline.model';

/** Maps a timeline item's subType/kind to an icon key + accent color. */
interface IconSpec { key: string; color: string; }

@Component({
  selector: 'app-timeline',
  imports: [DatePipe],
  templateUrl: './timeline.html',
  styleUrl: './timeline.scss'
})
export class Timeline {
  /** Date-grouped, newest-first data straight from the backend timeline endpoint. */
  readonly groups = input.required<TimelineGroup[]>();

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
