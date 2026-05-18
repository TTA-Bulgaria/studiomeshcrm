# Studio Mesh CRM — Product & User Guide

**Version:** 1.0  
**Last Updated:** May 2026  
**URL:** https://app.studiomeshcrm.com

---

## What is Studio Mesh CRM?

Studio Mesh CRM is an all-in-one operations platform built for digital marketing agencies. It replaces the spreadsheets, separate invoicing tools, and disconnected project trackers most agencies use — bringing clients, projects, sales, contracts, billing, time tracking, and ad performance into one place.

---

## Getting Started

### 1. Create Your Account

Go to https://app.studiomeshcrm.com/register

Fill in:
- Your full name
- Your agency name
- Email address
- Password (minimum 8 characters)

After registering, check your inbox for a verification email. Click the link to verify your account before logging in.

> If you don't receive the email within a few minutes, check your spam folder or use the **Resend verification email** link on the login page.

### 2. Complete Onboarding

On first login you'll be guided through a 4-step setup:

1. **About You** — Your job title and phone number
2. **Agency Details** — Industry, website, target monthly revenue, business address
3. **Branding** — Your brand colour and logo URL
4. **Launch** — Review and confirm

This takes about 2 minutes. You can update everything later in Settings.

### 3. Log In

Go to https://app.studiomeshcrm.com/login  
Enter your email and password.

After login you'll land on the **Dashboard**.

---

## Features

### Dashboard

The dashboard gives you a live snapshot of your agency:

- **Revenue** — Total invoiced this month
- **Active Projects** — Number of projects currently in progress
- **Open Leads** — Leads in your pipeline
- **Ad Spend** — Total managed spend across connected ad platforms
- **Recent activity** — Latest clients, invoices, and tasks

---

### Clients

**Where:** Left sidebar → Clients

Manage all your agency clients in one place.

#### Adding a Client

Click **New Client** and fill in:
- Company name and legal name
- Industry and priority tier (Low / Medium / High)
- VAT / Tax ID
- Business address
- Contacts — add multiple contacts per client, each tagged as **Commercial** or **Financial**

#### Client Dashboard

Click any client name to open their dedicated dashboard showing:
- All projects for that client
- Contract history and status
- Invoice summary (total billed, paid, outstanding)

---

### Projects

**Where:** Left sidebar → Projects

Projects are the core unit of work in Studio Mesh CRM. Each project is linked to a client.

#### Creating a Project

Click **New Project**, select the client, add a name and description.

#### Inside a Project

Each project has five tabs:

**Tasks**  
Kanban board with three columns: Todo → In Progress → Done  
- Add tasks with title, description, and priority
- Drag tasks between columns to update status
- Log time directly from any task card

**Timeline**  
Gantt chart view showing task durations and project schedule.

**Time Tracking**  
Log and view all billable hours on the project.  
Each entry records: hours, date, work description, and the team member who logged it.

**Ad Accounts**  
Link Facebook/Meta ad accounts to this project.  
Once linked, ad spend, impressions, clicks, and conversions sync automatically every day.

**Team**  
Add team members to the project and see their time contributions.

---

### Leads

**Where:** Left sidebar → Leads

Track every sales opportunity from first contact to close.

#### Lead Fields

- **Contact details** — Name, company, email, phone
- **Source** — Facebook, Google, Website, Referral, or Manual
- **Service interest** — Development, Marketing, Staffing, or Other
- **Pipeline stage** — Discovery, Proposal, or Negotiation
- **Deal value** — Estimated contract value
- **Close probability** — Percentage likelihood to close
- **Status** — New, Contacted, Qualified, or Lost

#### Moving a Lead Forward

Update a lead's status using the dropdown on each lead card. When a lead is accepted and an offer is created, the system can automatically generate a project and draft contract.

---

### Offers

**Where:** Left sidebar → Offers

Create and track proposals sent to clients.

The offers board is a **drag-and-drop Kanban** with four columns:

| Column | Meaning |
|--------|---------|
| Draft | Offer being prepared |
| Sent | Sent to client |
| Accepted | Client accepted |
| Rejected | Client declined |

Drag an offer card to move it through the workflow.

#### Creating an Offer

Click **New Offer** and fill in:
- Client
- Offer title
- Line items (description + amount for each service)

The system calculates the total automatically.

#### Tracking Client Views

Each offer card shows whether the client has opened it. Once a client views the offer link, the card updates to **Viewed**.

#### When an Offer is Accepted

Accepting an offer automatically triggers:
1. A new **Project** is created
2. A **Contract** draft is generated
3. Default **Tasks** are added to the project

---

### Contracts

**Where:** Left sidebar → Contracts

Manage service agreements with your clients including digital signatures.

#### Contract Statuses

| Status | Meaning |
|--------|---------|
| Draft | Being prepared |
| Sent | Sent to client for signature |
| Signed | Client has signed |
| Completed | Contract period ended |
| Cancelled | Cancelled before completion |
| Archived | Stored for records |

#### Billing Terms

Each contract supports two billing components:

- **Base Retainer** — Fixed monthly fee
- **Success Fee** — Choose one:
  - None
  - Fixed per lead (e.g. $50 per qualified lead)
  - Revenue share % (e.g. 10% of revenue generated)

#### Sending for Signature

1. Set contract status to **Sent**
2. The contract gets a unique portal link
3. Share the link with your client
4. The client opens the portal, reviews the terms, and signs digitally
5. The contract updates to **Signed** automatically

#### Generating an Invoice from a Contract

On any signed contract, click **Generate Invoice** to create an invoice pre-filled with the contract's billing terms.

---

### Invoices

**Where:** Left sidebar → Invoices

Track all billing and payments.

#### Invoice Statuses

| Status | Meaning |
|--------|---------|
| Draft | Not yet sent |
| Sent | Sent to client |
| Partially Paid | Some payment received |
| Paid | Fully paid |
| Overdue | Past due date, unpaid |
| Cancelled | Cancelled |

#### Recording a Payment

1. Open an invoice
2. Click **Record Payment**
3. Enter: amount, payment date, method (Bank Transfer / Credit Card / PayPal / Stripe / Cash), reference number, and optional notes
4. The invoice status updates automatically based on the amount paid

#### Currencies

Invoices use the currency set in your agency Settings. Supported: USD, EUR, GBP, NGN.

---

### Tasks

**Where:** Left sidebar → Tasks

A global view of all tasks across all projects.

- Three columns: **Todo**, **In Progress**, **Done**
- Each task shows: title, project, priority
- Log time from any task card
- Tasks can also be managed inside each individual project

---

### Analytics

**Where:** Left sidebar → Analytics

Agency-wide advertising performance and profitability.

#### Metrics Shown

- **Total Ad Spend** — Sum across all connected ad accounts
- **Total Impressions**
- **Total Clicks**
- **CPC** — Cost per click
- **ROAS** — Return on ad spend
- **Project Profitability** — Per-project breakdown of spend vs results

#### AI Business Insights *(Alpha)*

An experimental section that generates recommendations based on your agency's performance data.

> Note: This feature is in early access. Insights are informational and should be reviewed before acting on them.

---

### Integrations

**Where:** Left sidebar → Integrations

Connect advertising platforms to pull real-time performance data.

#### Meta Ads (Facebook & Instagram) — Available

1. Go to Integrations
2. Click **Connect** on Meta Ads
3. You'll be redirected to Facebook to authorise the connection
4. Once connected, go to any Project → Ad Accounts tab to link a specific ad account
5. Metrics sync automatically every day

**What syncs:** Daily spend, impressions, clicks, conversions per ad account.

If your access token expires, the platform will prompt you to reconnect.

#### Google Ads — Coming Soon
#### TikTok Ads — Coming Soon

---

### Settings

**Where:** Left sidebar → Settings

#### Agency Profile
Update your agency's legal name, VAT/Tax ID, billing email, business address, and currency.

#### Branding
Update your brand colour and logo URL. These appear across your workspace.

#### Password
Change your account password. Requires your current password.

#### Automation
View and manually trigger background automations:

| Automation | What it does |
|------------|-------------|
| Offer Accepted | Auto-creates Project, Contract draft, and Tasks |
| Monthly Billing | Generates invoices for all active retainer contracts |
| Ad Metrics Sync | Pulls latest data from all connected ad platforms |

Monthly billing and ad metrics sync run automatically on schedule. Use the manual trigger buttons to run them immediately if needed.

---

## Account & Security

### Forgot Password

1. Go to https://app.studiomeshcrm.com/forgot-password
2. Enter your email address
3. Check your inbox for a reset link
4. The link is valid for **1 hour**
5. Set your new password

### Email Verification

New accounts must verify their email before logging in. If you're stuck on the verification step:
- Check your spam/junk folder
- Click **Resend verification email** on the login page

### Session Security

- Login sessions use short-lived JWT tokens with automatic refresh
- Sessions are invalidated on password change

---

## User Roles

| Role | Access |
|------|--------|
| Admin | Full access to all features and settings |
| Sales Manager | Leads, Offers, Clients |
| Project Manager | Projects, Tasks, Time Tracking |
| Accountant | Invoices, Contracts, Payments |

Role assignments are managed by your workspace Admin.

---

## Frequently Asked Questions

**Can I have multiple users in my agency?**  
Yes. Your workspace is multi-user. Contact your Admin to add team members and assign roles.

**Can clients sign contracts without a Studio Mesh account?**  
Yes. The contract portal link works for anyone — clients do not need an account to view or sign.

**What happens if I reject an offer by mistake?**  
You can update the offer status back to Sent or Draft by dragging it on the Kanban board.

**How often do ad metrics sync?**  
Automatically once per day. You can also trigger a manual sync from Settings → Automation or from the Ad Accounts tab inside a project.

**Can I use multiple currencies?**  
One currency per workspace, set in Settings. All invoices use that currency.

**What happens when a contract is signed?**  
The contract status updates to Signed, the signed date is recorded, and the client's IP is logged. You can then generate invoices from the contract.

---

## Support

For help or to report an issue: heyoogun@gmail.com  
Production app: https://app.studiomeshcrm.com
