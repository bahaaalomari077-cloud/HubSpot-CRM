# توثيق Bitrix24 الكامل — مرجع تفصيلي

> مُستخرَج من حساب حقيقي عبر REST API الداخلي + اختبار حي للسلوك
> المصدر: `b24-glmtze.bitrix24.com` · التاريخ: 21 سبتمبر 2026
> **الطريقة:** `/rest/<method>.json?sessid=<session>` — جلسة المتصفح تفتح الـ API بالكامل

---

## المحتويات

**الجزء الأول — البيانات**
1. [ملخص الكيانات](#1-ملخص-الكيانات)
2. [كيانات الـ CRM](#2-كيانات-الـ-crm)
3. [Activity و Timeline](#3-activity-و-timeline)
4. [المنتجات والبنود](#4-المنتجات-والبنود)
5. [Tasks — 65 حقل](#5-tasks--65-حقل)
6. [Sale — الطلبات والدفع والشحن](#6-sale--الطلبات-والدفع-والشحن)
7. [Catalog و Inventory](#7-catalog-و-inventory)
8. [المستخدمون والأقسام](#8-المستخدمون-والأقسام)
9. [Disk — الملفات](#9-disk--الملفات)

**الجزء الثاني — القواميس والسلوك**
10. [القواميس والـ Enums](#10-القواميس-والـ-enums)
11. [السلوك الحي المُختبَر](#11-السلوك-الحي-المُختبَر)

**الجزء الثالث — الواجهات**
12. [خريطة الوحدات](#12-خريطة-الوحدات)
13. [ما لم أستطع توثيقه](#13-ما-لم-أستطع-توثيقه)

---

## 1) ملخص الكيانات

| # | الكيان | الحقول | الـ API |
|---|---|---|---|
| 1 | Lead | 58 | `crm.lead.fields` |
| 2 | Deal | 48 (REST) / 55 (item) / **72 (فعلي)** | `crm.deal.fields` |
| 3 | Contact | 50 | `crm.contact.fields` |
| 4 | Company | 57 | `crm.company.fields` |
| 5 | Quote | 43 | `crm.quote.fields` |
| 6 | SmartInvoice | 41 | `crm.item.fields` (31) |
| 7 | **Activity** | **44** | `crm.activity.fields` |
| 8 | TimelineComment | 7 | `crm.timeline.comment.fields` |
| 9 | Product | 22 | `crm.product.fields` |
| 10 | ProductRow | 21 | `crm.productrow.fields` |
| 11 | **Task** | **65** | `tasks.task.getFields` |
| 12 | Order | 43 | `sale.order.getFields` |
| 13 | Payment | 44 | `sale.payment.getFields` |
| 14 | Shipment | 45 | `sale.shipment.getFields` |
| 15 | BasketItem | 24 | `sale.basketitem.getFields` |
| 16 | Store | 19 | `catalog.store.getFields` |
| 17 | Section | 10 | `catalog.section.getFields` |
| 18 | User | 63 | `user.fields` |
| 19 | DiskFile | 16 | `disk.file.getFields` |
| 20 | DiskFolder | 14 | `disk.folder.getFields` |
| 21 | Currency | 13 | `crm.currency.fields` |
| 22 | Status | 11 | `crm.status.fields` |
| 23 | UserField | 19 | `crm.userfield.fields` |
| 24 | DealCategory | 5 | `crm.dealcategory.fields` |

**المجموع: 24 كيان · ~800 حقل**

---

## 2) كيانات الـ CRM

### 2.1 Lead — 58 حقل

```
ID, TITLE, HONORIFIC, NAME, SECOND_NAME, LAST_NAME, BIRTHDATE,
COMPANY_TITLE, SOURCE_ID, SOURCE_DESCRIPTION,
STATUS_ID, STATUS_DESCRIPTION, STATUS_SEMANTIC_ID, POST,

-- العنوان (11 حقل!)
ADDRESS, ADDRESS_2, ADDRESS_CITY, ADDRESS_POSTAL_CODE, ADDRESS_REGION,
ADDRESS_PROVINCE, ADDRESS_COUNTRY, ADDRESS_COUNTRY_CODE, ADDRESS_LOC_ADDR_ID,

-- المال
CURRENCY_ID, OPPORTUNITY, IS_MANUAL_OPPORTUNITY,

-- الحالة والوصول
OPENED, COMMENTS, HAS_PHONE, HAS_EMAIL, HAS_IMOL,

-- الملكية والتدقيق
ASSIGNED_BY_ID, CREATED_BY_ID, MODIFY_BY_ID, MOVED_BY_ID,
DATE_CREATE, DATE_MODIFY, MOVED_TIME,

-- العلاقات
COMPANY_ID, CONTACT_ID, CONTACT_IDS, IS_RETURN_CUSTOMER, DATE_CLOSED,

-- المصدر الخارجي
ORIGINATOR_ID, ORIGIN_ID,

-- التتبع التسويقي
UTM_SOURCE, UTM_MEDIUM, UTM_CAMPAIGN, UTM_CONTENT, UTM_TERM,

-- النشاط
LAST_ACTIVITY_TIME, LAST_ACTIVITY_BY, LAST_COMMUNICATION_TIME,

-- متعدد القيم
PHONE, EMAIL, WEB, IM, LINK
```

> **ملاحظة:** العنوان مُفكَّك إلى **11 عموداً منفصلاً**، لا jsonb. هذا يجعل الفلترة الجغرافية سهلة لكنه يُثقل الجدول.

### 2.2 Deal — 48 حقل (REST)

```
ID, TITLE, TYPE_ID, CATEGORY_ID,
STAGE_ID, PREVIOUS_STAGE_ID, STAGE_SEMANTIC_ID,
IS_NEW, IS_RECURRING, IS_RETURN_CUSTOMER, IS_REPEATED_APPROACH,
PROBABILITY,

-- المال
CURRENCY_ID, OPPORTUNITY, IS_MANUAL_OPPORTUNITY, TAX_VALUE,

-- العلاقات
COMPANY_ID, MYCOMPANY_ID, CONTACT_ID, CONTACT_IDS, QUOTE_ID, LEAD_ID,

-- التواريخ
BEGINDATE, CLOSEDATE, DATE_CREATE, DATE_MODIFY, MOVED_TIME,

-- الحالة
OPENED, CLOSED, COMMENTS,

-- الملكية
ASSIGNED_BY_ID, CREATED_BY_ID, MODIFY_BY_ID, MOVED_BY_ID,

-- المصدر
SOURCE_ID, SOURCE_DESCRIPTION, ADDITIONAL_INFO, LOCATION_ID,
ORIGINATOR_ID, ORIGIN_ID,

UTM_SOURCE, UTM_MEDIUM, UTM_CAMPAIGN, UTM_CONTENT, UTM_TERM,
LAST_ACTIVITY_TIME, LAST_ACTIVITY_BY, LAST_COMMUNICATION_TIME
```

> ⚠️ **ثلاث أرقام مختلفة لنفس الكيان:**
> - `crm.deal.fields` (REST القديم) → **48**
> - `crm.item.fields` (الجديد) → **55**
> - **السجل الفعلي المُنشأ** → **72**
>
> **لا تثق بأي endpoint وحده. أنشئ سجلاً واقرأه.**

### 2.3 Contact — 50 حقل

نفس بنية الـ Lead تقريباً، مع:
- `PHOTO` (ملف)
- `TYPE_ID` (نوع جهة الاتصال)
- `EXPORT` (مشمول في التصدير)
- `COMPANY_IDS` (شركات متعددة)
- `ORIGIN_VERSION` (نسخة المصدر)

### 2.4 Company — 57 حقل ⭐ الأكبر

**الميزة الفريدة: عنوانان كاملان**

```
-- العنوان الفعلي (11 حقل)
ADDRESS, ADDRESS_2, ADDRESS_CITY, ADDRESS_POSTAL_CODE, ADDRESS_REGION,
ADDRESS_PROVINCE, ADDRESS_COUNTRY, ADDRESS_COUNTRY_CODE, ADDRESS_LOC_ADDR_ID,
ADDRESS_LEGAL,

-- العنوان المسجَّل قانونياً (9 حقول إضافية!)
REG_ADDRESS, REG_ADDRESS_2, REG_ADDRESS_CITY, REG_ADDRESS_POSTAL_CODE,
REG_ADDRESS_REGION, REG_ADDRESS_PROVINCE, REG_ADDRESS_COUNTRY,
REG_ADDRESS_COUNTRY_CODE, REG_ADDRESS_LOC_ADDR_ID,

-- خاص بالشركات
BANKING_DETAILS, INDUSTRY, EMPLOYEES, REVENUE, CURRENCY_ID,
IS_MY_COMPANY, LOGO, COMPANY_TYPE
```

> **20 عموداً للعناوين وحدها.** السبب: الفوترة القانونية تتطلب العنوان المسجَّل رسمياً، وقد يختلف عن مقر العمل.

### 2.5 Quote — 43 حقل

**الفريد فيه: لقطة مجمّدة من بيانات العميل**

```
CLIENT_TITLE, CLIENT_ADDR, CLIENT_CONTACT, CLIENT_EMAIL, CLIENT_PHONE,
CLIENT_TP_ID, CLIENT_TPA_ID
```

> **قرار تصميمي ذكي:** العرض يُجمّد بيانات العميل وقت إصداره. لو تغيّر عنوان الشركة لاحقاً، العرض القديم يحتفظ بالعنوان الذي أُرسل به فعلاً. **انسخ هذا** — المستندات المالية يجب ألا تتغير بأثر رجعي.

```
QUOTE_NUMBER, STATUS_ID, CONTENT, TERMS,
BEGINDATE, CLOSEDATE, ACTUAL_DATE (صالح حتى),
DEAL_ID, LEAD_ID, PERSON_TYPE_ID
```

---

## 3) Activity و Timeline

### 3.1 Activity — 44 حقل ⭐

هذا الجدول كان أكبر ثغرة، والآن موثّق بالكامل.

```sql
ID                      integer
OWNER_ID                integer          -- العنصر المالك
OWNER_TYPE_ID           crm_enum_ownertype
TYPE_ID                 crm_enum_activitytype  *مطلوب
PROVIDER_ID             string           -- CRM_CALL | CRM_EMAIL | REST_APP...
PROVIDER_TYPE_ID        string
PROVIDER_GROUP_ID       string
ASSOCIATED_ENTITY_ID    integer

SUBJECT                 string           *مطلوب
DESCRIPTION             string
DESCRIPTION_TYPE        crm_enum_contenttype

-- التوقيت
START_TIME              datetime
END_TIME                datetime
DEADLINE                datetime

-- الحالة
COMPLETED               char (Y/N)
STATUS                  crm_enum_activitystatus
PRIORITY                crm_enum_activitypriority
DIRECTION               crm_enum_activitydirection  -- وارد/صادر
IS_INCOMING_CHANNEL     char

-- الإشعارات
NOTIFY_TYPE             crm_enum_activitynotifytype
NOTIFY_VALUE            integer

-- الأشخاص
RESPONSIBLE_ID          user  *مطلوب
AUTHOR_ID               user
EDITOR_ID               user

-- التدقيق
CREATED                 datetime
LAST_UPDATED            datetime

-- النتيجة (7 حقول! نظام تقييم كامل)
RESULT_STATUS           integer
RESULT_STREAM           integer
RESULT_SOURCE_ID        string
RESULT_MARK             integer
RESULT_VALUE            double
RESULT_SUM              double
RESULT_CURRENCY_ID      string

-- الربط والاتصال
BINDINGS                crm_activity_binding
COMMUNICATIONS          crm_activity_communication  *مطلوب
FILES                   diskfile
WEBDAV_ELEMENTS         diskfile

LOCATION                string
SETTINGS                object
PROVIDER_PARAMS         object
PROVIDER_DATA           string
AUTOCOMPLETE_RULE       integer
ORIGIN_ID, ORIGINATOR_ID  string
```

**ثلاثة دروس معمارية:**

1. **`PROVIDER_ID`** — النشاط ليس نوعاً ثابتاً، بل **مزوّد قابل للتوسيع**. المكالمة والإيميل والمهمة كلها أنشطة بمزوّدين مختلفين. هذا يسمح لتطبيقات خارجية بإضافة أنواع أنشطة.

2. **حقول `RESULT_*` السبعة** — النشاط يحمل نتيجة مالية. مكالمة يمكن أن تُنتج مبلغاً. هذا يربط النشاط بالإيراد مباشرة.

3. **`BINDINGS` متعدد** — النشاط الواحد يرتبط بعدة عناصر CRM في آن. إيميل واحد قد يخص صفقتين وجهة اتصال.

### 3.2 TimelineComment — 7 حقول

```
ID, CREATED, ENTITY_ID *, ENTITY_TYPE *, AUTHOR_ID, COMMENT, FILES
```

بسيط عمداً — السجل يجب أن يبقى خفيفاً وسريع الكتابة.

---

## 4) المنتجات والبنود

### 4.1 Product — 22 حقل

```
ID, CATALOG_ID, NAME, CODE, DESCRIPTION, DESCRIPTION_TYPE,
PRICE, CURRENCY_ID, ACTIVE, SECTION_ID, SORT,
VAT_ID, VAT_INCLUDED, MEASURE, XML_ID,
PREVIEW_PICTURE, DETAIL_PICTURE,
DATE_CREATE, TIMESTAMP_X, MODIFIED_BY, CREATED_BY, PROPERTY_44
```

> `PROPERTY_44` — خاصية مخصصة مُنشأة في هذا الحساب. رقمها يدل على نظام خصائص ديناميكي.

### 4.2 ProductRow — 21 حقل

```
ID, OWNER_ID, OWNER_TYPE (D/Q/I), PRODUCT_ID, PRODUCT_NAME,
PRICE, PRICE_EXCLUSIVE, PRICE_NETTO, PRICE_BRUTTO,
QUANTITY,
DISCOUNT_TYPE_ID, DISCOUNT_RATE, DISCOUNT_SUM,
TAX_RATE, TAX_NAME, TAX_INCLUDED,
CUSTOMIZED, MEASURE_CODE, MEASURE_NAME, SORT, TYPE
```

**أربع صيغ سعر مختلفة** لكل بند:

| الحقل | المعنى |
|---|---|
| `PRICE` | السعر المُدخل |
| `PRICE_EXCLUSIVE` | بعد الخصم، قبل الضريبة |
| `PRICE_NETTO` | الصافي |
| `PRICE_BRUTTO` | الإجمالي مع الضريبة |

---

## 5) Tasks — 65 حقل

أكبر كيان مفرد في النظام، مع **قيم enum كاملة**.

### 5.1 الحقول الأساسية

```
ID (PK), PARENT_ID (مهام فرعية), TITLE *, DESCRIPTION,
GUID, XML_ID, SITE_ID
```

### 5.2 الحالة والأولوية — بالقيم الفعلية

```
STATUS (افتراضي: 2)
  2 = Pending          قيد الانتظار
  3 = In progress      قيد التنفيذ
  4 = Pending review   بانتظار المراجعة
  5 = Completed        مكتملة
  6 = Deferred         مؤجلة

PRIORITY (افتراضي: 1)
  0 = Low              منخفضة
  1 = Normal           عادية
  2 = High             عالية

MARK (التقييم)
  N = Negative         سلبي
  P = Positive         إيجابي
```

> **لاحظ:** `STATUS` يبدأ من **2** لا من 0 أو 1 — إرث من نسخ قديمة. لا تنسخ هذا الترقيم.

### 5.3 الأشخاص

```
CREATED_BY *        منشئ المهمة
RESPONSIBLE_ID *    المسؤول
ACCOMPLICES  array  المشاركون
AUDITORS     array  المراقبون
CHANGED_BY, STATUS_CHANGED_BY, CLOSED_BY
```

> **أربعة أدوار مختلفة** على المهمة الواحدة. نموذج صلاحيات غني.

### 5.4 التواريخ — 11 حقل

```
CREATED_DATE, CHANGED_DATE, STATUS_CHANGED_DATE, CLOSED_DATE,
ACTIVITY_DATE, VIEWED_DATE,
DATE_START, DEADLINE,
START_DATE_PLAN, END_DATE_PLAN,
EXCHANGE_MODIFIED
```

**التمييز المهم:** `DATE_START` (البدء الفعلي) مقابل `START_DATE_PLAN` (المخطط). ونفس الشي للنهاية. هذا يسمح بتقارير الانحراف عن الخطة.

### 5.5 الوقت والجهد

```
TIME_ESTIMATE       integer   الوقت المقدَّر
TIME_SPENT_IN_LOGS  integer   الوقت المسجَّل فعلياً
DURATION_PLAN       integer   المدة المخططة
DURATION_FACT       integer   المدة الفعلية
DURATION_TYPE       enum      secs|mins|hours|days|weeks|monts|years
MATCH_WORK_TIME     integer   مطابقة ساعات العمل
ALLOW_TIME_TRACKING enum Y/N
ADD_IN_REPORT       enum Y/N
```

> `"monts"` — خطأ إملائي في Bitrix نفسه (الصحيح months). موجود في الإنتاج.

### 5.6 التنظيم

```
GROUP_ID      integer   المشروع/المجموعة
STAGE_ID      integer   مرحلة الكانبان
SPRINT_ID     integer   السبرنت (Scrum)
BACKLOG_ID    integer   قائمة الانتظار (Scrum)
FLOW_ID       integer   التدفق (Flows)
CHAT_ID       integer   محادثة المهمة
FORUM_TOPIC_ID, FORUM_ID
SORTING       double    الترتيب اليدوي
```

> **Scrum مدمج في نفس جدول المهام** — لا جدول منفصل. `SPRINT_ID` و`BACKLOG_ID` أعمدة عادية.

### 5.7 الأعلام السلوكية

```
MULTITASK, NOT_VIEWED, REPLICATE, SUBORDINATE, FAVORITE,
ALLOW_CHANGE_DEADLINE, TASK_CONTROL,
IS_MUTED, IS_PINNED, IS_PINNED_IN_GROUP,
FORKED_BY_TEMPLATE_ID
```

### 5.8 الربط بالـ CRM ⭐

```
UF_CRM_TASK          type: crm        "CRM Items"
UF_TASK_WEBDAV_FILES type: disk_file  "Load files"
UF_MAIL_MESSAGE      type: mail_message
CHECKLIST            array
COMMENTS_COUNT, SERVICE_COMMENTS_COUNT, NEW_COMMENTS_COUNT
```

> **`UF_CRM_TASK` من نوع `crm`** — حقل واحد يربط المهمة بأي عنصر CRM (Lead/Deal/Contact/Company). هذا نوع حقل مخصص، لا FK عادي.

### 5.9 التكامل الخارجي

```
EXCHANGE_ID, EXCHANGE_MODIFIED, OUTLOOK_VERSION
```

تكامل مع Microsoft Exchange/Outlook مبني في الجدول الأساسي.

---

## 6) Sale — الطلبات والدفع والشحن

### 6.1 Order — 43 حقل

```
id, accountNumber, lid, personTypeId, personTypeXmlId,
orderTopic, userId, responsibleId, companyId,

-- المال
price, currency, discountValue, taxValue,

-- الحالة
statusId, statusXmlId, dateStatus, empStatusId,
payed, deducted, canceled, marked,

-- الإلغاء والتعليم
dateCanceled, reasonCanceled, empCanceledId,
dateMarked, reasonMarked, empMarkedId,

-- القفل (منع التعديل المتزامن)
dateLock, lockedBy,

-- التواريخ
dateInsert, dateUpdate,

-- التكامل مع 1C (نظام محاسبة روسي)
id1c, version1c, updated1c, externalOrder,

comments, userDescription, additionalInfo,
affiliateId, recurringId, recountFlag, version, xmlId
```

> **نمط مهم:** كل حالة لها **ثلاثية**: العلم (`canceled`) + التاريخ (`dateCanceled`) + السبب (`reasonCanceled`) + من نفّذها (`empCanceledId`). انسخ هذا النمط — يجعل التدقيق كاملاً.

### 6.2 Payment — 44 حقل

```
id, orderId, accountNumber, sum, currency,
paid, datePaid, empPaidId,
dateBill, datePayBefore,

-- نظام الدفع
paySystemId, paySystemName, paySystemIsCash, paySystemXmlId,

-- استجابة بوابة الدفع (8 حقول)
psStatus, psStatusCode, psStatusDescription, psStatusMessage,
psSum, psCurrency, psInvoiceId, psResponseDate,

-- الإرجاع
isReturn, payReturnDate, payReturnNum, payReturnComment, empReturnId,

-- السند
payVoucherNum, payVoucherDate,

priceCod, marked, dateMarked, reasonMarked, empMarkedId,
responsibleId, dateResponsibleId, empResponsibleId, companyId,
comments, externalPayment, id1c, version1c, updated1c, xmlId
```

> **8 حقول لاستجابة بوابة الدفع** — كل ما ترسله البوابة يُحفظ. ضروري لحل النزاعات المالية.

### 6.3 Shipment — 45 حقل

```
id, orderId, accountNumber,
deliveryId, deliveryName, deliveryXmlId, system,

-- السعر
priceDelivery, basePriceDelivery, customPriceDelivery,
discountPrice, currency,

-- حالة الشحن
allowDelivery, dateAllowDelivery, empAllowDeliveryId,
deducted, dateDeducted, empDeductedId, reasonUndoDeducted,
canceled, dateCanceled, empCanceledId,
statusId, statusXmlId,

-- التتبع (4 حقول)
trackingNumber, trackingStatus, trackingDescription, trackingLastCheck,

-- المستندات
deliveryDocNum, deliveryDocDate,

marked, dateMarked, reasonMarked, empMarkedId,
responsibleId, dateResponsibleId, empResponsibleId, companyId,
comments, externalDelivery, dateInsert,
id1c, version1c, updated1c, xmlId
```

### 6.4 BasketItem — 24 حقل

```
id, orderId, productId, productXmlId, catalogXmlId, name,
price, basePrice, customPrice, discountPrice, currency,
quantity, measureCode, measureName,
vatRate, vatIncluded,
weight, dimensions,
canBuy, type, sort, dateInsert, dateUpdate, xmlId
```

---

## 7) Catalog و Inventory

### 7.1 Store — 19 حقل

```
id, title, active, code, xmlId, sort,
address, description, email, phone, schedule,
gpsN, gpsS,                    -- إحداثيات GPS
imageId, issuingCenter,        -- مركز تسليم
userId, modifiedBy,
dateCreate, dateModify
```

> `issuingCenter` — علم يميّز المستودع عن نقطة الاستلام.

**من الحساب الفعلي:** مستودع واحد باسم افتراضي، عنوانه `944 5th Avenue`، بإحداثيات `40.768`.

### 7.2 Section — 10 حقول

```
id, iblockId, iblockSectionId, name, code,
description, descriptionType, active, sort, xmlId
```

> `iblockId` — نظام Information Blocks، البنية التحتية لـ Bitrix للمحتوى المنظَّم.

---

## 8) المستخدمون والأقسام

### 8.1 User — 63 حقل

```
-- الهوية
ID, XML_ID, ACTIVE, NAME, LAST_NAME, SECOND_NAME, TITLE, EMAIL,
USER_TYPE,

-- الجلسة
LAST_LOGIN, DATE_REGISTER, TIME_ZONE, IS_ONLINE,
TIMESTAMP_X, LAST_ACTIVITY_DATE,

-- شخصي (15 حقل)
PERSONAL_GENDER, PERSONAL_PROFESSION, PERSONAL_WWW, PERSONAL_BIRTHDAY,
PERSONAL_PHOTO, PERSONAL_ICQ, PERSONAL_PHONE, PERSONAL_FAX,
PERSONAL_MOBILE, PERSONAL_PAGER, PERSONAL_STREET, PERSONAL_CITY,
PERSONAL_STATE, PERSONAL_ZIP, PERSONAL_COUNTRY, PERSONAL_MAILBOX,
PERSONAL_NOTES,

-- العمل (16 حقل)
WORK_PHONE, WORK_COMPANY, WORK_POSITION, WORK_DEPARTMENT,
WORK_WWW, WORK_FAX, WORK_PAGER, WORK_STREET, WORK_MAILBOX,
WORK_CITY, WORK_STATE, WORK_ZIP, WORK_COUNTRY,
WORK_PROFILE, WORK_LOGO, WORK_NOTES,

-- حقول مخصصة
UF_DEPARTMENT        ← الربط بالقسم (مصفوفة)
UF_EMPLOYMENT_DATE, UF_TIMEMAN, UF_PHONE_INNER, UF_DISTRICT,
UF_INTERESTS, UF_SKILLS, UF_WEB_SITES,
UF_SKYPE, UF_SKYPE_LINK, UF_ZOOM,
UF_XING, UF_LINKEDIN, UF_FACEBOOK, UF_TWITTER
```

> `ICQ` و`PAGER` — حقول من التسعينات لا تزال في المنتج. مثال على تراكم الإرث.

### 8.2 Department

```
department.get → [{ ID: 1, NAME: "Bitrix24", SORT: 500 }]
```

قسم واحد جذري. الهيكل شجري عبر `PARENT` في النسخ الكاملة.

---

## 9) Disk — الملفات

### 9.1 DiskFile — 16 حقل

```
ID, NAME, TYPE, CODE, STORAGE_ID, PARENT_ID,
FILE_ID, SIZE, GLOBAL_CONTENT_VERSION,
CREATE_TIME, UPDATE_TIME, DELETE_TIME,
CREATED_BY, UPDATED_BY, DELETED_BY, DELETED_TYPE
```

### 9.2 DiskFolder — 14 حقل

```
ID, NAME, TYPE, CODE, STORAGE_ID, REAL_OBJECT_ID, PARENT_ID,
CREATE_TIME, UPDATE_TIME, DELETE_TIME,
CREATED_BY, UPDATED_BY, DELETED_BY, DELETED_TYPE
```

> **الملف والمجلد نفس البنية** — `TYPE` يميّزهما، و`PARENT_ID` يبني الشجرة. و`REAL_OBJECT_ID` للاختصارات (shortcuts).

**التخزينات الفعلية في الحساب:**
```
ID 1: مساحة المستخدم (entity_type: user)
ID 3: Shared drive (entity_type: common)
```

**الحذف الناعم مبني في البنية:** `DELETE_TIME` + `DELETED_BY` + `DELETED_TYPE`.

---

## 10) القواميس والـ Enums

### 10.1 أنواع القواميس الـ13

```
STATUS                    = Lead stages          [له semantic]
DEAL_STAGE                = Deal Stage           [له semantic]
QUOTE_STATUS              = Estimate stages      [له semantic]
SMART_INVOICE_STAGE_10    = Invoice stages       [له semantic]
SMART_DOCUMENT_STAGE_12   = Document stages      [له semantic]
SOURCE                    = Sources
CONTACT_TYPE              = Contact Type
COMPANY_TYPE              = Company Type
DEAL_TYPE                 = Deal Type
EMPLOYEES                 = Employees
INDUSTRY                  = Industry
HONORIFIC                 = Salutations
CALL_LIST                 = Call statuses
```

**5 قواميس لها `SEMANTIC_INFO`** — أي أنها آلات حالة، لا قوائم بسيطة:

```json
STATUS (Lead): {
  "START_FIELD": "NEW",
  "FINAL_SUCCESS_FIELD": "CONVERTED",
  "FINAL_UNSUCCESS_FIELD": "JUNK",
  "FINAL_SORT": 0
}
```

> **تصميم مهم:** كل آلة حالة تُعرِّف بداية ونهاية ناجحة ونهاية فاشلة. هذا يجعل الـ pipeline قابلاً للتهيئة بالكامل دون كود.

### 10.2 أنواع الأنشطة

```
1 = Meeting       اجتماع
2 = Call          مكالمة
3 = Task          مهمة
4 = E-mail        بريد
5 = Activity      نشاط عام
6 = User action   إجراء مستخدم
```

### 10.3 حالات الأنشطة

```
1 = Pending                 قيد الانتظار
2 = Completed               مكتمل
3 = Completed automatically مكتمل تلقائياً
```

> التمييز بين الإكمال اليدوي والتلقائي — مفيد للتقارير.

### 10.4 أنواع المالكين — الجدول الكامل

| ID | الرمز | الاسم |
|---|---|---|
| 1 | `L` | Lead |
| 2 | `D` | Deal |
| 3 | `C` | Contact |
| 4 | `CO` | Company |
| 5 | `I` | Invoice (قديم) |
| 7 | `Q` | Estimate |
| 8 | `RQ` | Details |
| 31 | `SI` | Invoice (جديد) |
| 36 | `DO` | Document |
| 39 | `SBD` | Company document |

> **الأرقام غير متتالية** (5 ثم 7 ثم 8 ثم 31 ثم 36 ثم 39) — دليل على تطور تدريجي. احجز نطاقات في تصميمك.

### 10.5 العملات

```
USD [الأساس], EUR, CNY, BRL, INR
```

**Currency — 13 حقل:**
```
CURRENCY, BASE, AMOUNT, AMOUNT_CNT, SORT, DATE_UPDATE,
LID, LANG, FULL_NAME, FORMAT_STRING,
DEC_POINT, THOUSANDS_SEP, DECIMALS
```

> `FORMAT_STRING` و`DEC_POINT` و`THOUSANDS_SEP` — التنسيق مخزَّن مع العملة، لا في الواجهة. **انسخ هذا.**

### 10.6 Status — 11 حقل

```
ID, ENTITY_ID, STATUS_ID, SORT, NAME, NAME_INIT,
SYSTEM, CATEGORY_ID, COLOR, SEMANTICS, EXTRA
```

> `SYSTEM` — يمنع حذف المراحل الأساسية. `NAME_INIT` — الاسم الأصلي قبل تعديل المستخدم.

### 10.7 UserField — 19 حقل

```
ID, ENTITY_ID, FIELD_NAME, USER_TYPE_ID, XML_ID, SORT,
MULTIPLE, MANDATORY,
SHOW_FILTER, SHOW_IN_LIST, EDIT_IN_LIST, IS_SEARCHABLE,
EDIT_FORM_LABEL, LIST_COLUMN_LABEL, LIST_FILTER_LABEL,
ERROR_MESSAGE, HELP_MESSAGE, LIST, SETTINGS
```

> **ثلاث تسميات منفصلة** لنفس الحقل: في النموذج، في العمود، في الفلتر. تفصيل يُحدث فرقاً في تجربة الاستخدام.

---

## 11) السلوك الحي المُختبَر

أنشأتُ صفقة حقيقية (ID #2)، نقلتها، أضفت بنداً، ثم حذفتها. **نتائج مُشاهَدة لا مُستنتَجة.**

### 11.1 الإنشاء — 28 حقل يُملأ تلقائياً

أرسلتُ 5 حقول. النظام ملأ 28:

```
begindate  = تاريخ اليوم
closedate  = اليوم + 7 أيام        ← افتراضي مدمج
typeId     = "SALE"
stageId    = "NEW"
stageSemanticId = "P"
isNew      = true
opened     = true
companyId = 0, contactId = 0       ← صفر لا NULL
searchContent = <ROT13>
```

### 11.2 آلة حالة المراحل

| النقلة | semantic | closed | closedate |
|---|---|---|---|
| NEW → PREPARATION | `P` | false | بلا تغيير |
| PREPARATION → EXECUTING | `P` | false | بلا تغيير |
| **EXECUTING → WON** | **`S`** | **true** | **← تاريخ اليوم** |

**ثلاثة سلوكيات تلقائية عند الإغلاق:**
1. `stageSemanticId`: `P` → `S`
2. `closed` → `true`
3. `closedate` يُعاد ضبطه لتاريخ الإغلاق الفعلي

### 11.3 إعادة حساب المبلغ ⭐

```
قبل:  opportunity = 1234.56  (يدوي)
       ↓ أضفت بند: 500 × 3، ضريبة 16%
بعد:  opportunity = 1500     ← أُعيد حسابه!
      taxValue    = 206.90
      isManualOpportunity = false
```

**بمجرد وجود بند، يصبح المبلغ محسوباً ويُلغى الإدخال اليدوي.**

### 11.4 دليل مشكلة الـ double

```
priceExclusive = 431.03448276   ← 11 رقماً عشرياً
```

انحراف عائم موثّق. **استخدم `numeric`.**

### 11.5 searchContent بـ ROT13

```
"2 GRFG — Pynhqr fpurzn nhqvg"
 = "2 TEST — Claude schema audit"
```

عمود بحث مُسطَّح مُشفَّر. الفكرة صحيحة، التنفيذ غريب. استخدم `tsvector`.

---

## 12) خريطة الوحدات

| الوحدة | المسار | الحالة | المحتوى |
|---|---|---|---|
| CRM — Deals | `/crm/deal/kanban/` | ✅ | Kanban · List · Activities · Calendar |
| CRM — Contacts | `/crm/contact/list/` | ✅ | جدول + اتصال جماعي + WhatsApp |
| CRM — Leads | `/crm/lead/list/` | 🔒 | مقفل بالباقة |
| CRM — Quotes | `/crm/quote/list/` | 🔒 | مقفل بالباقة |
| Tasks | `/company/personal/user/1/tasks/` | ✅ | Tasks · Projects · Flows · Scrum · Gantt · Planner |
| Booking | `/booking/` | ✅ | موارد · حجوزات · قائمة انتظار |
| Inventory | `/shop/documents/` | ✅ | أوامر · تحويلات · شطب · مستودعات |
| Marketing | `/marketing/` | ✅ | إيميل/SMS · Google Ads · Facebook |
| BI Builder | `/bi/dashboard/` | ✅ | 30 لوحة / 5 مجموعات |
| Contact Center | `/contact_center/` | ✅ | 18 قناة |
| e-Signature | `/sign/` | ✅ | توقيع SMS/إيميل/WhatsApp |
| Sites & Stores | `/sites/` | 🔒 | مقفل |

### قنوات Contact Center الـ18

```
Mail · Telephony · Call tracking · Website widget · Website form
Live chat · WhatsApp · Viber · Telegram · Apple Messages for Business
Facebook · Facebook Comments · Instagram Direct · Bitrix24 Network
Instant WhatsApp · Edna.io WhatsApp · Chat bots · Voice bots
```

**إعداد القناة المفتوحة الفعلي:**
```json
{
  "LINE_NAME": "Open Channel",
  "CRM": "Y",
  "CRM_CREATE": "lead",        ← ينشئ Lead تلقائياً
  "CRM_FORWARD": "Y",
  "CRM_CHAT_TRACKER": "Y",
  "QUEUE_TIME": "60"
}
```

### لوحات BI — 30 لوحة

| المجموعة | العدد |
|---|---|
| CRM | 12 |
| Tasks and flows | 8 |
| User profile | 4 |
| Sites and stores | 4 |
| Automation | 2 |

### أعمدة قائمة المهام

```
Name · Active · Deadline · Created by · Assignee · Status · Project · Flow
Created on · Modified on · Completed on · Estimated time · Track time spent
Rating · Actual duration · Tags · Dependency type · Start date · Due date
→ Lead · Contact · Company · Deal · CRM Items
```

### أعمدة Inventory

```
Name · Inventory document # · Type · Status · Modified on
Responsible person · Vendor · Total · Warehouses
```

---

## 13) ما لم أستطع توثيقه

**بصراحة كاملة** — هذه المناطق بقيت مغلقة:

| المنطقة | السبب |
|---|---|
| `crm.automation.trigger.list` | `Access denied! Application context required` — يتطلب تطبيق مُسجَّل |
| `placement.list`, `event.get` | نفس السبب — سياق تطبيق |
| `bizproc.*` (محرك سير العمل) | `Method not found!` — الوحدة غير مثبّتة |
| `calendar.*.getFields` | `Method not found!` |
| `sign.b2e.document.list` | `Method not found!` |
| `biconnector.dashboard.list` | الـ controller موجود، الـ method لا |
| `im.chat.list` | `Method not found!` |
| مخطط Booking | لا endpoints عامة |
| محرر الأتمتة البصري | الصفحة تعيد التوجيه |
| صفحة تفاصيل الصفقة | خطأ JS في الـ SPA |
| **داخل الوحدات** | وثّقت الشاشة الأولى فقط لكل وحدة — لم أفتح كل نموذج وإعداد |

### تقييم صادق للتغطية

| الطبقة | التغطية |
|---|---|
| **مخططات البيانات** (24 كيان) | ✅ **ممتازة** — من الـ API مباشرة |
| **القواميس والـ enums** | ✅ **كاملة** |
| **سلوك الـ CRM** | ✅ **مُختبَر حياً** |
| **سطح الوحدات** | ⚠️ الشاشة الأولى فقط |
| **داخل الوحدات** | ❌ عشرات الشاشات لم تُفتح |
| **الأتمتة وسير العمل** | ❌ محمية |

> **الخلاصة:** التوثيق قوي جداً على **طبقة البيانات** — وهي الأهم لبناء نظام مماثل. لكنه **ليس** توثيقاً لكل شاشة في Bitrix24؛ المنتج فيه آلاف الشاشات، وهذا يحتاج أسابيع.

---

## ملاحظات على المصدر

- **الطريقة:** `/rest/<method>.json?sessid=<BX.bitrix_sessid()>` — جلسة المتصفح المُصادَقة تفتح REST API الداخلي دون webhook
- **الاختبار الحي:** صفقة ID #2 أُنشئت، نُقلت عبر 3 مراحل، أُضيف بند، ثم **حُذفت**
- **الحساب رجع لحالته الأصلية** — صفر صفقات، مُتحقَّق منه
- الحساب **مخفَّض الباقة**: Leads/Quotes/Sites مقفلة في الواجهة، لكن الـ API يُرجع مخططاتها
- `crm.controller.type.list` → `{"types":[]}` — لا عمليات ذكية مخصصة
- `tasks.api.scrum.sprint.list` → `[]` — لا سبرنتات
- `landing.landing.getList` → `[]` — لا صفحات هبوط
- المستودع الوحيد: عنوان افتراضي `944 5th Avenue`
