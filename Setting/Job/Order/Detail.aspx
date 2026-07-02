<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Setting_Job_Order_Detail" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Detail Job Order" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .info-box { padding: 12px 14px; border-radius: 10px; background: #f8f9fa; border: 1px solid #eee; }
        .info-box label { font-size: 12px; letter-spacing: .3px; text-transform: uppercase; }
        .info-box .fw-semibold { font-size: 15px; }
        .card { transition: all .2s ease-in-out; }
        .card:hover { transform: translateY(-2px); box-shadow: 0 8px 25px rgba(0,0,0,0.08); }
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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/job">Job</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/job/order">Order</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row mb-3" runat="server" id="divError">
            <div class="col-12">
                <div class="alert alert-danger">
                    <span runat="server" id="msgError"></span>
                </div>
            </div>
        </section>
        <section class="row justify-content-center">
            <div class="col-12">
                <div class="card shadow-sm border-0 rounded-3">
                    <div class="card-header bg-white border-bottom-0 pt-4 pb-0">
                        <h5 class="mb-0 fw-semibold text-dark">Job Sheet Data</h5>
                    </div>
                    <div class="card-body pt-4">
                        <div class="row g-4">
                            <div class="col-12 col-md-4">
                                <div class="info-box">
                                    <label class="form-label text-muted mb-1">Order #</label>
                                    <div class="fw-semibold fs-6 text-dark">
                                        <asp:Label runat="server" ID="lblOrderId"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-12 col-md-4">
                                <div class="info-box">
                                    <label class="form-label text-muted mb-1">Job Number</label>
                                    <div class="fw-semibold fs-6 text-dark">
                                        <asp:Label runat="server" ID="lblJobNumber"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-12 col-md-4">
                                <div class="info-box">
                                    <label class="form-label text-muted mb-1">Work Order</label>
                                    <div class="fw-semibold fs-6 text-dark">
                                        <asp:Label runat="server" ID="lblWorkOrder"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-12 col-md-4">
                                <div class="info-box">
                                    <label class="form-label text-muted mb-1">Converted</label>
                                    <div class="fw-semibold fs-6 text-dark">
                                        <asp:Label runat="server" ID="lblConverted"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-12 col-md-6">
                                <div class="info-box">
                                    <label class="form-label text-muted mb-1">Note</label>
                                    <div class="fw-semibold fs-6 text-dark">
                                        <asp:Label runat="server" ID="lblJobNote"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-12 col-md-2">
                                <div class="info-box">
                                    <label class="form-label text-muted mb-1">Status</label>
                                    <div class="fw-semibold fs-6 text-dark">
                                        <asp:Label runat="server" ID="lblActive" CssClass="badge rounded-pill bg-success text-white"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <section class="row">
            <div class="col-12">
                <div class="card">
                    <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="card-content">
                                <div class="card-header">
                                    <div class="row mb-2">
                                        <div class="col-12 col-sm-12 col-lg-7 mb-2">
                                            <h4 class="card-title">Job Order</h4>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-5">
                                            <asp:DropDownList runat="server" ID="ddlJobSheet" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlJobSheet_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                    
                                </div>
                                <div class="card-body">
                                    <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center"></asp:GridView>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </section>
    </div>

    <div id="loadingOverlay" style="display:none; position:fixed; top:0; left:0; width:100%; height:100%; background:rgba(255,255,255,.5); z-index:99999;">
        <div class="position-absolute top-50 start-50 translate-middle">
            <div class="card shadow">
                <div class="card-body text-center">
                    <div class="spinner-border"></div>
                    <div class="mt-2">Loading...</div>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
    </div>

    <script type="text/javascript">
        window.addEventListener("pageshow", function () {
            var loading = document.getElementById("loadingOverlay");
            if (loading) loading.style.display = "none";
        });
        function initUpdatePanelLoading() {
            if (typeof Sys === "undefined") return;
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "block";
            });
            prm.add_endRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "none";
                initChoices();
            });
        }
        function initChoices() {
            document.querySelectorAll("select.choices").forEach(function (el) {
                if (el.choices) {
                    el.choices.destroy();
                }
                el.choices = new Choices(el, {
                    searchEnabled: true,
                    itemSelectText: '',
                    shouldSort: false
                });
            });
        }
        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
            initChoices();
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>