<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Job_Sheet_Detail_Add" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Job Sheet Detail" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/job/sheet">Sheet</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-8 mb-2">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Sheed Detail Form</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="form-body">
                                    <div class="row">
                                        <div class="col-12 form-group">
                                            <label class="form-label">Job Sheet</label>
                                            <asp:DropDownList runat="server" ID="ddlJobSheet" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12 form-group">
                                            <label class="form-label">Name</label>
                                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-4 form-group">
                                            <label class="form-label">Type</label>
                                            <asp:DropDownList runat="server" ID="ddlType" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="Field" Text="Field"></asp:ListItem>
                                                <asp:ListItem Value="Custom" Text="Custom"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-8 form-group" runat="server" id="divFormulaField">
                                            <label class="form-label">Formula</label>
                                            <asp:DropDownList runat="server" ID="ddlFormula" TextMode="MultiLine" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                        <div class="col-8 form-group" runat="server" id="divFormulaCustom">
                                            <label class="form-label">Formula</label>
                                            <asp:TextBox runat="server" ID="txtFormula" TextMode="MultiLine" CssClass="form-control" Height="100px" autocomplete="off" style="resize:none;"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-4 form-group">
                                            <label class="form-label">Type</label>
                                            <asp:DropDownList runat="server" ID="ddlType2" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType2_SelectedIndexChanged">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="Field" Text="Field"></asp:ListItem>
                                                <asp:ListItem Value="Custom" Text="Custom"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-8 form-group" runat="server" id="divFormulaField2">
                                            <label class="form-label">Formula</label>
                                            <asp:DropDownList runat="server" ID="ddlFormula2" TextMode="MultiLine" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                        <div class="col-8 form-group" runat="server" id="divFormulaCustom2">
                                            <label class="form-label">Formula</label>
                                            <asp:TextBox runat="server" ID="txtFormula2" TextMode="MultiLine" CssClass="form-control" Height="100px" autocomplete="off" style="resize:none;"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-4 form-group">
                                            <label class="form-label">Type</label>
                                            <asp:DropDownList runat="server" ID="ddlType3" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType3_SelectedIndexChanged">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="Field" Text="Field"></asp:ListItem>
                                                <asp:ListItem Value="Custom" Text="Custom"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-8 form-group" runat="server" id="divFormulaField3">
                                            <label class="form-label">Formula</label>
                                            <asp:DropDownList runat="server" ID="ddlFormula3" TextMode="MultiLine" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                        <div class="col-8 form-group" runat="server" id="divFormulaCustom3">
                                            <label class="form-label">Formula</label>
                                            <asp:TextBox runat="server" ID="txtFormula3" TextMode="MultiLine" CssClass="form-control" Height="100px" autocomplete="off" style="resize:none;"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-4 form-group">
                                            <label class="form-label">Type</label>
                                            <asp:DropDownList runat="server" ID="ddlType4" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType4_SelectedIndexChanged">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="Field" Text="Field"></asp:ListItem>
                                                <asp:ListItem Value="Custom" Text="Custom"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-8 form-group" runat="server" id="divFormulaField4">
                                            <label class="form-label">Formula</label>
                                            <asp:DropDownList runat="server" ID="ddlFormula4" TextMode="MultiLine" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                        <div class="col-8 form-group" runat="server" id="divFormulaCustom4">
                                            <label class="form-label">Formula</label>
                                            <asp:TextBox runat="server" ID="txtFormula4" TextMode="MultiLine" CssClass="form-control" Height="100px" autocomplete="off" style="resize:none;"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-4 form-group">
                                            <label class="form-label">Type</label>
                                            <asp:DropDownList runat="server" ID="ddlType5" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType5_SelectedIndexChanged">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="Field" Text="Field"></asp:ListItem>
                                                <asp:ListItem Value="Custom" Text="Custom"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-8 form-group" runat="server" id="divFormulaField5">
                                            <label class="form-label">Formula</label>
                                            <asp:DropDownList runat="server" ID="ddlFormula5" TextMode="MultiLine" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                        <div class="col-8 form-group" runat="server" id="divFormulaCustom5">
                                            <label class="form-label">Formula</label>
                                            <asp:TextBox runat="server" ID="txtFormula5" TextMode="MultiLine" CssClass="form-control" Height="100px" autocomplete="off" style="resize:none;"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-4 form-group">
                                            <label class="form-label">Type</label>
                                            <asp:DropDownList runat="server" ID="ddlType6" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType6_SelectedIndexChanged">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="Field" Text="Field"></asp:ListItem>
                                                <asp:ListItem Value="Custom" Text="Custom"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-8 form-group" runat="server" id="divFormulaField6">
                                            <label class="form-label">Formula</label>
                                            <asp:DropDownList runat="server" ID="ddlFormula6" TextMode="MultiLine" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                        <div class="col-8 form-group" runat="server" id="divFormulaCustom6">
                                            <label class="form-label">Formula</label>
                                            <asp:TextBox runat="server" ID="txtFormula6" TextMode="MultiLine" CssClass="form-control" Height="100px" autocomplete="off" style="resize:none;"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mt-3" runat="server" id="divError">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgError"></span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
    </div>

    <script type="text/javascript">
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>