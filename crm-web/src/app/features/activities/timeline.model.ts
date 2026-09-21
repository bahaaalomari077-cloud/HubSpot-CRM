export type TimelineItemKind = 'Activity' | 'Message';

export interface TimelineItem {
  id: string;
  kind: TimelineItemKind;
  occurredAt: string;
  title: string;
  detail?: string | null;
  /** Sub-type for icon selection: Call, Meeting, Email, WhatsApp, Sms, … */
  tag: string;
  direction?: string | null;   // messages: Inbound / Outbound
  isCompleted?: boolean | null; // activities
}

export interface TimelineGroup {
  date: string;      // yyyy-MM-dd
  label: string;     // "Today" | "Yesterday" | "March 4, 2026"
  items: TimelineItem[];
}
