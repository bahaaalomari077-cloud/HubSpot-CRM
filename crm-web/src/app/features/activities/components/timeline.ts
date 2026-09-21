import { Component, input } from '@angular/core';
import { DatePipe } from '@angular/common';
import { TimelineGroup, TimelineItem } from '../timeline.model';

/** Maps a timeline item's tag/kind to an icon key + accent color. */
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

  /** Pick an icon + color for an item, keyed off its tag (falls back on kind). */
  iconFor(item: TimelineItem): IconSpec {
    switch (item.tag) {
      case 'Call':      return { key: 'call',     color: '#2f6df6' };
      case 'Meeting':   return { key: 'meeting',  color: '#6b54d3' };
      case 'Task':      return { key: 'task',     color: '#0f9b8e' };
      case 'Email':     return { key: 'email',    color: '#d97706' };
      case 'WhatsApp':  return { key: 'whatsapp', color: '#25d366' };
      case 'Sms':       return { key: 'sms',      color: '#0ea5e9' };
      case 'Telegram':  return { key: 'telegram', color: '#229ed9' };
      case 'LiveChat':  return { key: 'chat',     color: '#8b5cf6' };
      default:
        return item.kind === 'Message'
          ? { key: 'chat', color: '#0f9b8e' }
          : { key: 'task', color: '#2f6df6' };
    }
  }
}
