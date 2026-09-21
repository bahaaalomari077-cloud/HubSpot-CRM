# مخطط بناء نظام CRM

> مُستخرَج من حساب Bitrix24 حقيقي عبر الـ API الداخلي، مع **اختبار حي** لسلوك النظام
> المصدر: `b24-glmtze.bitrix24.com` · التاريخ: 21 سبتمبر 2026

---

## المحتويات

1. [ما يحتويه النظام فعلياً](#1-ما-يحتويه-النظام-فعلياً)
2. [السلوك الحي المُختبَر](#2-السلوك-الحي-المُختبَر)
3. [نظام الأنواع](#3-نظام-الأنواع)
4. [مخطط قاعدة البيانات](#4-مخطط-قاعدة-البيانات)
5. [جداول SQL](#5-جداول-sql)
6. [بنود المنتجات](#6-بنود-المنتجات)
7. [خريطة الوحدات الكاملة](#7-خريطة-الوحدات-الكاملة)
8. [التقنيات المختارة](#8-التقنيات-المختارة)
9. [الصلاحيات](#9-الصلاحيات)
10. [ترتيب البناء](#10-ترتيب-البناء)
11. [ما لا يجب نسخه](#11-ما-لا-يجب-نسخه)
12. [ما بقي غير موثّق](#12-ما-بقي-غير-موثّق)

---

## 1) ما يحتويه النظام فعلياً

| الكيان | Type ID | حقول الـ schema | حقول السجل الفعلي | النوع |
|---|---|---|---|---|
| `Lead` | 1 | 52 | — | أساسي |
| **`Deal`** | **2** | **55** | **72** ⚠️ | **أساسي** |
| `Contact` | 3 | 45 | — | أساسي |
| `Company` | 4 | 42 | — | أساسي |
| `Quote` | 7 | 45 | — | مساند |
| `SmartInvoice` | 31 | 41 | — | مساند |
| `ProductRow` | — | 22 | 21 | بنود |

**مجموع حقول الـ schema: 302**

> ⚠️ **فجوة الـ schema:** الـ API يُعلن 55 حقلاً للصفقة، لكن السجل الفعلي المُنشأ يحتوي **72 حقلاً**. 17 حقلاً إضافياً موجودة في التخزين ولا يكشفها الـ `fields` endpoint — منها `searchContent` و `priceAccount` و `entityTypeId`. **لا تثق بـ `fields` وحده؛ أنشئ سجلاً واقرأه.**

### الاكتشاف الحاسم

الكيانات الستة تتشارك **~30 حقلاً متطابقاً**:

```
assignedById · createdTime · updatedTime · stageId · opportunity
currencyId · opened · comments · sourceId · observers
utmSource · utmMedium · utmCampaign · utmContent · utmTerm
lastCommunicationCallTime · lastCommunicationEmailTime
lastCommunicationImolTime · lastCommunicationWebformTime
```

> **Bitrix24 ليس ستة جداول منفصلة.** إنه تجريد واحد لـ "عنصر" مع مجموعات حقول لكل نوع فوقه.

**لماذا يهم؟** ستة جداول متشابهة = كل ميزة (الإسناد، نقل المراحل، البحث، الصلاحيات، التدقيق) تُكتب ست مرات وتتباعد. الجدول المشترك يكتبها مرة.

### الـ Pipeline الحقيقي — بالمعرّفات الفعلية

```
┌──────────┐  ┌──────────────┐  ┌────────────┐  ┌─────────────┐  ┌──────────────┐
│   New    │─▶│Create papers │─▶│  Invoice   │─▶│ In progress │─▶│Final invoice │
│  "NEW"   │  │"PREPARATION" │  │"PREPAYMENT │  │ "EXECUTING" │  │  "FINAL_     │
│          │  │              │  │  _INVOICE" │  │             │  │   INVOICE"   │
│ #39A8EF  │  │   #2FC6F6    │  │  #55D0E0   │  │   #47E4C2   │  │   #FFA900    │
│ semantic │  │  semantic    │  │  semantic  │  │  semantic   │  │  semantic    │
│   = P    │  │    = P       │  │    = P     │  │     = P     │  │     = P      │
└──────────┘  └──────────────┘  └────────────┘  └─────────────┘  └──────┬───────┘
                                                                         │
                                                      ┌──────────────────┴─────────────┐
                                                      ▼                                ▼
                                               ┌─────────────┐                 ┌─────────────┐
                                               │  Deal won   │                 │  Deal lost  │
                                               │   "WON"     │                 │   "LOSE"    │
                                               │  #3BC864    │                 │  #F1361D    │
                                               │ semantic=S  │                 │ semantic=F  │
                                               └─────────────┘                 └─────────────┘
```

**رموز الـ semantic** (مُتحقَّق منها بالاختبار الحي):

| الرمز | المعنى | `closed` |
|---|---|---|
| `P` | Process — قيد التنفيذ | `false` |
| `S` | Success — نجاح | `true` |
| `F` | Failure — فشل | `true` |

---

## 2) السلوك الحي المُختبَر

أنشأتُ صفقة حقيقية، نقلتها عبر المراحل، أضفت بند منتج، ثم حذفتها. هذه النتائج **مُشاهَدة لا مُستنتَجة**.

### أ) ما يملؤه النظام تلقائياً عند الإنشاء

أرسلتُ 5 حقول فقط. النظام ملأ **28 حقلاً**:

```
id, createdTime, updatedTime, createdBy, updatedBy, assignedById,
opened=true, companyId=0, contactId=0, mycompanyId=0,
categoryId=0, stageId="NEW", stageSemanticId="P", isNew=true,
typeId="SALE", taxValue=0, begindate=اليوم, closedate=+7 أيام,
webformId=0, searchContent=..., movedBy, movedTime,
lastActivityBy, lastActivityTime, entityTypeId
```

**دروس للتطبيق:**
- `begindate` = تاريخ اليوم، `closedate` = **اليوم + 7 أيام** تلقائياً
- العلاقات الفارغة تُخزَّن كـ `0` لا `NULL` — اختيار تصميمي مقصود
- `isNew=true` يُضبط عند الإنشاء ويصبح `false` عند أول تعديل

### ب) `searchContent` — فهرس البحث المُشفَّر

```
searchContent = "2 GRFG — Pynhqr fpurzn nhqvg (fnsr gb qryrg..."
                 ↑ ROT13 لـ "TEST — Claude schema audit (safe to delet"
```

Bitrix يبني عمود بحث مُسطَّح يجمع العنوان والمعرّف والحقول المرتبطة، **مُشفَّراً بـ ROT13**. هذا ليس أماناً — بل حيلة لمنع محركات البحث الداخلية من مطابقة محتوى لا ينبغي مطابقته حرفياً.

> **للتطبيق:** الفكرة صحيحة (عمود بحث مُسطَّح مُحدَّث بـ trigger)، لكن استخدم `tsvector` من Postgres بدل ROT13.

### ج) آلة حالة المراحل — مُتحقَّق منها

| النقلة | `stageSemanticId` | `closed` | `closedate` | `previousStageId` |
|---|---|---|---|---|
| NEW → PREPARATION | `P` | false | 09/28 (بلا تغيير) | `NEW` |
| PREPARATION → EXECUTING | `P` | false | 09/28 (بلا تغيير) | `PREPARATION` |
| **EXECUTING → WON** | **`S`** | **true** | **09/21 ← تغيّر لليوم** | `EXECUTING` |

**ثلاثة سلوكيات تلقائية عند الإغلاق:**
1. `stageSemanticId` يتحوّل من `P` إلى `S`
2. `closed` يصبح `true`
3. **`closedate` يُعاد ضبطه لتاريخ اليوم** — تاريخ الإغلاق الفعلي يحلّ محل المتوقَّع

و`previousStageId` يُحفظ في كل نقلة — يكفي لتتبّع التراجع دون قراءة الـ timeline.

### د) إعادة حساب المبلغ — الأهم

```
قبل:  opportunity = 1234.56   (يدوي)
       ↓  أضفت بنداً: 500 × 3 قطع، ضريبة 16%
بعد:  opportunity = 1500      ← أُعيد حسابه تلقائياً!
       taxValue    = 206.90
       isManualOpportunity = false
```

**بمجرد وجود بند منتج واحد، يصبح مبلغ الصفقة محسوباً ويُلغى الإدخال اليدوي.** العلم `isManualOpportunity` هو المُبدِّل بين الوضعين.

**للتطبيق:** لا تسمح بأن يكون المبلغ عموداً بسيطاً قابلاً للكتابة. اجعله:
```
IF is_manual_amount THEN القيمة المُدخلة
ELSE SUM(بنود المنتجات)
```

### هـ) دليل مشكلة الـ `double`

```
price = 500, tax = 16%
priceExclusive = 431.03448276   ← 11 رقماً عشرياً
```

هذا ناتج قسمة عائمة مُخزَّنة كما هي. مع آلاف البنود، الجمع ينحرف. **هذا دليل مباشر على أن `numeric` ضروري.**

---

## 3) نظام الأنواع

عبر 302 حقل، يستخدم Bitrix24 **22 نوعاً** فقط.

| نوع Bitrix | الاستخدامات | المقابل في Postgres | ملاحظة |
|---|---|---|---|
| `string` | 61 | `text` | لا تحدد حجم varchar |
| `datetime` | 46 | `timestamptz` | دائماً مع المنطقة الزمنية |
| `user` | 32 | `bigint` → users | الملكية والتدقيق |
| `boolean` | 28 | `boolean` | افتراضي false، not null |
| `crm_status` | 17 | `bigint` → status_ref | قاموس قابل للتعديل |
| `text` | 14 | `text` | نص طويل |
| `crm_contact` | 14 | FK / جدول ربط | مفرد ومتعدد معاً |
| `integer` | 12 | `bigint` | |
| `crm_company` | 10 | FK | |
| `date` | 9 | `date` | بدون وقت |
| **`double`** | **8** | **`numeric(18,2)`** | ⚠️ **لا تستخدم float** |
| `crm_currency` | 5 | `char(3)` | ISO 4217 |
| `crm_category` | 4 | FK → pipelines | |
| `crm_webform` | 4 | FK | نسب المصدر |
| `crm_lead/deal/quote/entity` | 8 | FK متعدد الأشكال | type id + row id |
| `crm_multifield` | 3 | جدول فرعي | هاتف/إيميل متعدد |
| `location` | 3 | `jsonb` | عنوان منظّم |
| `file` | 2 | FK → files | مفتاح التخزين |
| `char` | 2 | `char(1)` | `Y`/`N` — إرث قديم |

---

## 4) مخطط قاعدة البيانات

```
                         ┌───────────────┐
                         │   pipeline    │
                         │  id · name    │
                         └───────┬───────┘
                                 │ 1
                                 ▼ N
                         ┌───────────────┐
                         │     stage     │
                         │ name · color  │
                         │ semantic P/S/F│
                         └───────┬───────┘
                                 │ 1
                                 ▼ N
     ┌──────────────────────────────────────────────────┐
     │                    crm_item                      │
     │           (الجدول الأساسي — القلب)                │
     │    entity_type_id + id  ·  ~30 عمود مشترك         │
     └─┬───────┬─────────┬─────────┬─────────┬─────────┬┘
       │1     │1        │1        │1        │1        │N
       ▼N     ▼N        ▼N        ▼N        ▼N        ▼1
  ┌────────┐┌────────┐┌────────┐┌────────┐┌────────┐┌───────┐
  │multi   ││product ││activity││timeline││binding ││ users │
  │field   ││_row    ││        ││        ││        ││       │
  ├────────┤├────────┤├────────┤├────────┤├────────┤├───────┤
  │kind    ││price   ││type    ││type    ││from_id ││name   │
  │ PHONE  ││quantity││ call   ││ created││to_id   ││dept   │
  │ EMAIL  ││discount││ meeting││ stage_ ││        ││       │
  │ IM     ││taxRate ││ task   ││ changed││  M:N   ││       │
  │value   ││measure ││due_at  ││author  ││        ││       │
  │is_prim ││        ││is_done ││data    ││        ││       │
  └────────┘└───┬────┘└────────┘└────────┘└────────┘└───────┘
   تلفونات      │      مهام لها   سجل ثابت   ربط       الموظفون
   وإيميلات     │      موعد       للتاريخ    العناصر
                │
                └──▶ يُعيد حساب opportunity + taxValue في crm_item
                     (إلا إذا is_manual_amount = true)

                         ┌───────────────┐
                         │  status_ref   │  (قواميس قابلة للتعديل)
                         │ source · type │
                         │   industry    │
                         └───────────────┘
```

### Timeline مقابل Activity — افصل بينهما

| | **Activity** | **Timeline** |
|---|---|---|
| **ماذا** | شيء مجدول يجب أن يفعله أحد | شيء حدث بالفعل |
| **يحمل** | موعد استحقاق، مالك، علامة إنجاز | طابع زمني، فاعل، بيانات |
| **يتغير** | نعم — يُنجَز، يُؤجَّل | لا — سجل ثابت append-only |
| **يُشغّل** | عدادات "متأخر" | سجل التدقيق |

دمجهما يُنتج جدولاً هو طابور عمل وسجل في آن، وسيئ في الاثنين.

---

## 5) جداول SQL

### الجدول الأساسي

```sql
CREATE TABLE crm_item (
  id              bigserial,
  entity_type_id  smallint    NOT NULL,   -- 1 lead, 2 deal, 3 contact…
  tenant_id       bigint      NOT NULL,
  title           text,

  -- الموقع في الـ pipeline
  pipeline_id     bigint      REFERENCES pipeline,
  stage_id        bigint      REFERENCES stage,
  stage_semantic  char(1)     NOT NULL DEFAULT 'P',  -- P process | S success | F failure
  previous_stage_id bigint,                          -- Bitrix يحفظه، مفيد للتراجع
  is_closed       boolean     NOT NULL DEFAULT false,
  is_new          boolean     NOT NULL DEFAULT true,

  -- المال
  amount          numeric(18,2) NOT NULL DEFAULT 0,
  tax_value       numeric(18,2) NOT NULL DEFAULT 0,
  currency_id     char(3)     NOT NULL DEFAULT 'USD',
  amount_base     numeric(18,2),   -- محوّل، للتجميع عبر العملات
  is_manual_amount boolean    NOT NULL DEFAULT false,  -- ← المُبدِّل المهم

  -- التواريخ
  begin_date      date        NOT NULL DEFAULT CURRENT_DATE,
  close_date      date        NOT NULL DEFAULT (CURRENT_DATE + 7),  -- سلوك Bitrix

  -- الملكية والوصول
  assigned_by_id  bigint      NOT NULL REFERENCES users,
  is_open         boolean     NOT NULL DEFAULT true,

  -- التدقيق
  created_by      bigint      NOT NULL,
  created_at      timestamptz NOT NULL DEFAULT now(),
  updated_by      bigint,
  updated_at      timestamptz NOT NULL DEFAULT now(),
  moved_by        bigint,
  moved_at        timestamptz,

  -- مُفكّك للترتيب في القوائم
  last_activity_at   timestamptz,
  last_comm_call_at  timestamptz,
  last_comm_email_at timestamptz,

  -- البحث (بديل searchContent المُشفَّر بـ ROT13)
  search_vector   tsvector,

  -- النسب والحقول المخصصة
  source_id       bigint      REFERENCES status_ref,
  utm             jsonb,      -- 5 حقول UTM في عمود واحد
  custom          jsonb,      -- الحقول المخصصة

  deleted_at      timestamptz,

  PRIMARY KEY (entity_type_id, id)
);
```

### الفهارس

```sql
-- الفهرس الذي تحتاجه كل استعلامات الـ Kanban
CREATE INDEX ON crm_item
  (tenant_id, entity_type_id, pipeline_id, stage_id)
  INCLUDE (amount, assigned_by_id)
  WHERE deleted_at IS NULL;

-- البحث النصي
CREATE INDEX ON crm_item USING GIN (search_vector);

-- الحقول المخصصة
CREATE INDEX ON crm_item USING GIN (custom jsonb_path_ops);

-- لوحات "المتأخر" و"اليوم"
CREATE INDEX ON crm_item (tenant_id, assigned_by_id, close_date)
  WHERE is_closed = false AND deleted_at IS NULL;
```

### قيد يمنع خطأً شائعاً

```sql
-- لا يمكن أن تكون الصفقة مغلقة والـ semantic قيد التنفيذ
ALTER TABLE crm_item ADD CONSTRAINT stage_semantic_consistency
  CHECK ( (stage_semantic = 'P') = (is_closed = false) );
```

### نمط الـ Multifield

```sql
CREATE TABLE multifield (
  id          bigserial PRIMARY KEY,
  entity_type smallint  NOT NULL,
  entity_id   bigint    NOT NULL,
  kind        text      NOT NULL,   -- PHONE | EMAIL | IM | WEB
  value_type  text      NOT NULL,   -- WORK | MOBILE | HOME | FAX
  value       text      NOT NULL,
  is_primary  boolean   NOT NULL DEFAULT false,
  sort        int       NOT NULL DEFAULT 100
);

CREATE INDEX ON multifield (entity_type, entity_id);
CREATE INDEX ON multifield (kind, value);  -- كشف التكرار + المكالمات الواردة
```

> الفهرس الثاني هو ما يجعل "رقم مجهول يتصل — من هذا؟" يُحَل في أجزاء من الثانية.

**انسخ هذا النمط حرفياً** — كل CRM يبدأ بـ `phone1`, `phone2`, `phone3` يندم لاحقاً.

---

## 6) بنود المنتجات

22 حقلاً، مُستخرجة من `crm.item.productrow.fields` ومُختبَرة حياً.

```sql
CREATE TABLE product_row (
  id            bigserial PRIMARY KEY,
  owner_type    char(1)   NOT NULL,   -- D=deal Q=quote I=invoice
  owner_id      bigint    NOT NULL,
  product_id    bigint,               -- 0 = بند حر بلا كتالوج
  product_name  text      NOT NULL,

  price         numeric(18,2) NOT NULL,
  quantity      numeric(12,3) NOT NULL DEFAULT 1,

  -- الخصم
  discount_type_id smallint NOT NULL DEFAULT 2,  -- 1=مبلغ 2=نسبة
  discount_rate    numeric(8,4) NOT NULL DEFAULT 0,
  discount_sum     numeric(18,2) NOT NULL DEFAULT 0,

  -- الضريبة
  tax_rate      numeric(6,3),
  tax_included  boolean   NOT NULL DEFAULT false,

  -- محسوبة (Bitrix يخزنها؛ يمكن جعلها generated)
  price_exclusive numeric(18,2),   -- السعر بعد الخصم قبل الضريبة
  price_netto     numeric(18,2),
  price_brutto    numeric(18,2),

  -- وحدة القياس
  measure_code  int,               -- 796 = قطعة (معيار روسي GOST)
  measure_name  text,

  sort          int NOT NULL DEFAULT 0
);

CREATE INDEX ON product_row (owner_type, owner_id);
```

### قاعدة إعادة الحساب — مُختبَرة حياً

```sql
-- عند أي تغيير في البنود، أعد حساب مبلغ العنصر المالك
UPDATE crm_item SET
  amount    = (SELECT COALESCE(SUM(price * quantity - discount_sum), 0)
               FROM product_row WHERE owner_id = ... AND owner_type = 'D'),
  tax_value = (SELECT COALESCE(SUM(...), 0) FROM product_row WHERE ...)
WHERE is_manual_amount = false;   -- ← احترم الإدخال اليدوي
```

> ⚠️ **ملاحظة:** `measure_code = 796` معيار روسي (GOST). استخدم UN/CEFACT أو معيارك المحلي.

---

## 7) خريطة الوحدات الكاملة

فتحتُ كل وحدة فعلياً. **معظمها متاح** — الأقفال محصورة وليست عامة.

| الوحدة | المسار | الحالة | ما بداخلها |
|---|---|---|---|
| **CRM — Deals** | `/crm/deal/kanban/` | ✅ مفتوح | Kanban · List · Activities · Calendar |
| **CRM — Contacts** | `/crm/contact/list/` | ✅ مفتوح | جدول + اتصال جماعي + WhatsApp |
| CRM — Leads | `/crm/lead/list/` | 🔒 مقفل | "you have downgraded" |
| CRM — Quotes | `/crm/quote/list/` | 🔒 مقفل | مقفل بالباقة |
| **Tasks** | `/company/personal/user/1/tasks/` | ✅ مفتوح | Tasks · Projects · **Flows** · **Scrum** · Gantt · Planner |
| **Booking** | `/booking/` | ✅ مفتوح | موارد · حجوزات · قائمة انتظار · تقويم يوم/أسبوع |
| **Inventory** | `/shop/documents/` | ✅ مفتوح | أوامر بيع · تحويلات · شطب · مستودعات · موردون |
| **Marketing** | `/marketing/` | ✅ مفتوح | حملات إيميل/SMS · Google Ads · Facebook · Sales Boost |
| **BI Builder** | `/bi/dashboard/` | ✅ مفتوح | **30 لوحة جاهزة** في 5 مجموعات |
| **Contact Center** | `/contact_center/` | ✅ مفتوح | **18 قناة** (انظر أدناه) |
| **e-Signature** | `/sign/` | ✅ مفتوح | توقيع عبر SMS/إيميل/WhatsApp · قوالب |
| Sites & Stores | `/sites/` | 🔒 مقفل | مقفل بالباقة |
| Automation | `/crm/deal/automation/0/` | ⚠️ أعاد التوجيه | لم يُفتح المحرر |

### قنوات Contact Center الـ 18

```
Mail · Telephony · Call tracking · Website widget · Website form
Live chat · WhatsApp · Viber · Telegram · Apple Messages for Business
Facebook · Facebook Comments · Instagram Direct · Bitrix24 Network
Instant WhatsApp · Edna.io WhatsApp · Chat bots · Voice bots
```

### لوحات BI الجاهزة (30 لوحة / 5 مجموعات)

| المجموعة | العدد |
|---|---|
| CRM dashboards | 12 |
| Tasks and flows | 8 |
| User profile | 4 |
| Sites and stores | 4 |
| Automation | 2 |

### 🔑 اكتشاف معماري: المهام مرتبطة بالـ CRM

أعمدة جدول المهام تشمل:

```
Name · Active · Deadline · Created by · Assignee · Status · Project · Flow
Created on · Modified on · Completed on · Estimated time · Track time spent
Rating · Actual duration · Tags · Dependency type · Start date · Due date
→ Lead · Contact · Company · Deal · CRM Items   ← ربط مباشر بالـ CRM
```

**المهمة ليست وحدة منفصلة** — إنها كيان يرتبط مباشرة بأي عنصر CRM. هذا يعني أن `binding` في مخططك يجب أن يدعم الربط عبر الوحدات، لا داخل الـ CRM فقط.

---

## 8) التقنيات المختارة

| الدور | الاختيار | السبب | البديل |
|---|---|---|---|
| **قاعدة البيانات** | PostgreSQL 16 | سلامة علائقية + `jsonb` للحقول المخصصة + `tsvector` للبحث + RLS للصلاحيات. تحتاجها كلها | ليس MongoDB — الـ CRM رسم بياني مترابط |
| **الخادم** | NestJS + TypeScript | النموذج المشترك يحتاج أنواعاً قوية عبر الكيانات | Go/Rust أسرع تنفيذاً، أبطأ بناءً |
| **ORM** | Drizzle | SQL-first، فيبقى عمل الفهارس سليماً | Prisma أسهل لكنه يخفي خطط الاستعلام |
| **الواجهة** | React + TanStack Query | اللوح مشكلة اتساق كاش. التحديث المتفائل هو نمط السحب والإفلات | Svelte/Vue جيدان؛ طبقة البيانات أهم |
| **السحب والإفلات** | dnd-kit | دعم لوحة المفاتيح وقارئ الشاشة افتراضياً | react-beautiful-dnd متوقف عن الصيانة |
| **البحث** | Postgres FTS ← Meilisearch | `tsvector` يكفي حتى ~مليون سجل | Elasticsearch أثقل من الحاجة |
| **اللحظي** | Centrifugo | Bitrix يدفع تحديثات حية ("server connection established") | WebSockets عادي يكفي حتى تشغّل أكثر من نسخة |
| **المهام والأتمتة** | BullMQ + Redis | قواعد الأتمتة مهام مؤجلة قابلة لإعادة المحاولة | pg-boss إن أردت تجنب Redis |
| **المال** | `numeric` + decimal.js | لا حسابات مالية بـ float في أي طبقة | — |

### محرك الأتمتة

```ts
// تغيير المرحلة يُصدِر حدثاً؛ محرك القواعد يستهلكه
await tx.insert(timeline).values({ itemId, type: 'stage_changed', from, to });
await queue.add('automation', { itemId, trigger: 'stage_entered', stageId: to },
  { attempts: 3, backoff: { type: 'exponential', delay: 2000 } });
```

> قاعدة تُرسل إيميلاً يجب ألا تجعل السحب يبدو بطيئاً، ويجب أن تنجو من فشل الإرسال.

---

## 9) الصلاحيات

كل كيان يحمل `assignedById` و `opened` و `observers` — **نموذج وصول كامل**، وإضافته لاحقاً تعني إعادة كتابة كل استعلام.

- **`assigned_by_id`** — المالك. الفلتر الأساسي "سجلاتي"
- **`is_open`** — مرئي للجميع. الحقل الوحيد الذي يعلّمه Bitrix **مطلوباً** في الكيانات الستة كلها، ويُضبط `true` تلقائياً
- **`observers`** — مستخدمون إضافيون لهم حق القراءة
- **التسلسل الإداري** — المدراء يرون سجلات مرؤوسيهم بشكل متعدٍّ

```sql
CREATE POLICY item_read ON crm_item FOR SELECT USING (
  tenant_id = current_tenant()
  AND (
    is_open
    OR assigned_by_id = current_user_id()
    OR assigned_by_id = ANY(subordinate_ids(current_user_id()))
    OR EXISTS (SELECT 1 FROM observer o
               WHERE o.item_id = crm_item.id AND o.user_id = current_user_id())
  )
);
```

---

## 10) ترتيب البناء

### 1. الأساس — **إلزامي**
المستأجرون، المستخدمون، الأقسام، المصادقة. `crm_item` مع Contact و Company. multifield. RLS من أول استعلام.
**يُسلِّم:** قاعدة جهات اتصال عاملة مع بحث وصلاحيات

### 2. الصفقات واللوح — **إلزامي**
Pipelines، مراحل، semantics (P/S/F). Kanban بسحب وإفلات متفائل. أحداث المراحل في الـ timeline.
**يُسلِّم:** الشاشة التي بدأ منها هذا المخطط

### 3. Timeline والأنشطة — **إلزامي**
سجل append-only. أنشطة مجدولة بمواعيد تُشغّل عدادات التأخير. تعليقات.
**يُسلِّم:** التاريخ والمتابعة

### 4. بنود المنتجات — **إلزامي** ⬆️
جدول `product_row` + منطق إعادة الحساب + احترام `is_manual_amount`.
**رُفعت من المرحلة 5** — الاختبار الحي أثبت أن المبلغ والبنود مترابطان بنيوياً، لا لاحقاً.
**يُسلِّم:** مبالغ صحيحة محسوبة

### 5. الحقول المخصصة — **مهم**
تعريفات لكل نوع، تخزين `jsonb`، نماذج ديناميكية، تكامل التصفية.
**يُسلِّم:** إعدادات خاصة بكل مستأجر

### 6. العروض والفواتير — **مهم**
أنواع كيانات جديدة على الجدول الأساسي — تُثبت أن التجريد كان صحيحاً. PDF، حالة الدفع.
**يُسلِّم:** من العرض إلى التحصيل

### 7. الأتمتة — **مهم**
قواعد محفّز/شرط/إجراء. مدعومة بطابور. ابدأ بثلاثة إجراءات قبل بناء محرر بصري.
**يُسلِّم:** الـ pipeline يدير نفسه

### 8. الاتصالات — **لاحقاً**
إيميل، هاتف، تحديد هوية الرقم الوارد عبر فهرس multifield. كل قناة مشروع تكامل حقيقي.
**يُسلِّم:** مركز الاتصال

### 9. التقارير — **لاحقاً**
قمع التحويل، معدل الفوز، التوقعات. نسخة قراءة أو جدول تجميع — لا تحليلات على جداول اللوح الحية.
**يُسلِّم:** عرض الإدارة

---

## 11) ما لا يجب نسخه

| # | المشكلة | الدليل | الحل |
|---|---|---|---|
| 1 | **المال كـ `double`** | `priceExclusive = 431.03448276` | `numeric(18,2)` |
| 2 | **5 أعمدة UTM لكل كيان** (25 عموداً) | من الـ schema | عمود `jsonb` + فهرس GIN |
| 3 | **`searchContent` بـ ROT13** | `"GRFG"` = `"TEST"` | `tsvector` من Postgres |
| 4 | **`0` بدل `NULL` للعلاقات** | `companyId=0, contactId=0` | `NULL` + FK حقيقي |
| 5 | **`char(1)` بـ `Y`/`N`** | `taxIncluded="N"` | `boolean` |
| 6 | **`measure_code=796`** (GOST روسي) | من البند المُنشأ | UN/CEFACT أو معيارك |
| 7 | **Lead ككيان منفصل** (52 حقلاً) | — | احذفه إن لم تحتج مرحلة تأهيل |
| 8 | **محرر أتمتة بصري أولاً** | — | 3 قواعد ثابتة تكفي 90% |

---

## 12) ما بقي غير موثّق

**التزاماً بالدقة** — هذه المناطق لم أتمكن من توثيقها:

| المنطقة | السبب |
|---|---|
| **مخطط جداول Activity / Timeline** | الـ controllers محمية: `crm.activity.fields` و `crm.timeline.fields` ترجع `NO_SUCH_ACTION`. التصميم المقترح **استنتاج معماري** |
| **الحقول المخصصة (User Fields)** | `crm.userfield.list` غير متاح. لا حقول مخصصة مُعرَّفة في هذا الحساب أصلاً |
| **محرر الأتمتة** | الصفحة أعادت التوجيه لعرض القائمة |
| **صفحة تفاصيل الصفقة** | فشلت بالتحميل — خطأ JS في الـ SPA |
| **Leads / Quotes / Sites** | مقفلة بالباقة (الـ schema متاح، الواجهة لا) |
| **البنية الداخلية الفعلية لـ Bitrix24** | تصميم الجداول أدناه **تصميمي أنا**، مبني على ملاحظات حقيقية — لكنه ليس نسخة من جداول Bitrix |

### حدود النطاق

هذا مخطط **نواة الـ CRM** (المراحل 1–7). الوحدات الأخرى — Tasks · Booking · Inventory · Marketing · BI · e-Signature · Contact Center — **كل واحدة منتج قائم بذاته**، ووثّقتُ سطحها فقط في القسم 7.

---

## ملاحظات على المصدر

- **حقول الـ schema** مقروءة من `crm.item.fields` و `crm.item.productrow.fields`
- **السلوك الحي** مُختبَر بإنشاء صفقة حقيقية (ID #2)، نقلها عبر 3 مراحل، إضافة بند منتج، ثم **حذفها**
- **الحساب رجع لحالته الأصلية** — صفر صفقات، تم التحقق بعد الحذف
- **ألوان المراحل** مقروءة من الـ DOM المباشر
- **الوحدات** مفتوحة واحدة واحدة بالمتصفح
- الحساب **مخفَّض الباقة**: Leads و Quotes و Sites مقفلة في الواجهة، لكن الـ API يُرجع مخططاتها
- `crm.controller.type.list` أرجع `{"types":[]}` — لا عمليات ذكية مخصصة معرَّفة
