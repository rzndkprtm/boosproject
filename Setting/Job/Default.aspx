<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Job_Default" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Job" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .dashboard-wrapper { display: grid; grid-template-columns: repeat(4, 1fr); gap: 20px; width: 100%; }
        .dashboard-card { position: relative; display: block; background: #fff; border-radius: 16px; padding: 24px; border: 1px solid #e5e5e5; box-shadow: 0 2px 8px rgba(0,0,0,0.06); transition: 0.2s ease; cursor: pointer; text-decoration: none; overflow: hidden; }
        .dashboard-card:hover { transform: translateY(-3px); box-shadow: 0 6px 18px rgba(0,0,0,0.12); }
        .dashboard-number { font-size: 38px; font-weight: 700; color: #222; line-height: 1; margin-bottom: 14px; }
        .dashboard-title { font-size: 20px; font-weight: 600; color: #222; margin-bottom: 6px; }
        .dashboard-desc { font-size: 13px; color: #777; line-height: 1.5; }
        @media (max-width: 1200px) { .dashboard-wrapper { grid-template-columns: repeat(2, 1fr); } }
        @media (max-width: 768px) { .dashboard-wrapper { grid-template-columns: 1fr; } }
    </style>

    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted"></p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting">Setting</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="dashboard-wrapper">
                <a href="/setting/job/sheet" class="dashboard-card">
                    <div class="dashboard-number"><%= GetSumData("JobSheets") %></div>
                    <div class="dashboard-title">Job Sheet</div>
                    <div class="dashboard-desc">Description</div>
                </a>
                <a href="/setting/job/order" class="dashboard-card">
                    <div class="dashboard-number"><%= GetSumData("OrderJobs") %></div>
                    <div class="dashboard-title">Job Order</div>
                    <div class="dashboard-desc">Description</div>
                </a>
            </div>
        </section>
    </div>
</asp:Content>