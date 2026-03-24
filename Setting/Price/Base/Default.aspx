<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Price_Base_Default" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.Master" Debug="true" Title="Price Base" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .grid-container {
            width: 100%;
            height: calc(100vh - 150px); /* sesuaikan dengan header page kamu */
            overflow: auto;
            border: 1px solid #ddd;
        }

        /* table full */
        .grid-container table {
            width: 100%;
            border-collapse: collapse;
        }

        /* header freeze */
        .grid-container th {
            position: sticky;
            top: 0;
            background: #f5f5f5;
            z-index: 3;
        }

        /* kolom pertama freeze */
        .grid-container td:first-child,
        .grid-container th:first-child {
            position: sticky;
            left: 0;
            background: #fff;
            z-index: 2;
        }

        /* pojok kiri atas */
        .grid-container th:first-child {
            z-index: 4;
        }

        /* biar tidak gepeng */
        .grid-container td,
        .grid-container th {
            white-space: nowrap;
            padding: 6px 10px;
        }
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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price">Price</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row" runat="server" id="divError">
            <div class="col-12">
                <div class="alert alert-danger">
                    <span runat="server" id="msgError"></span>
                </div>
            </div>
        </section>

        <section class="row">
            <div class="col-12 d-flex justify-content-end flex-wrap gap-2">
                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="Editable Page" />
            </div>
        </section>

        <section class="row mt-3">
            <div class="col-9">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-12 col-sm-12 col-lg-3">
                                    <div class="input-group">
                                        <label class="input-group-text">Category</label>
                                        <asp:DropDownList runat="server" ID="ddlCategory" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Sell" Text="Sell Price"></asp:ListItem>
                                            <asp:ListItem Value="Buy" Text="Buy Price"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-12 col-sm-12 col-lg-9">
                                    <div class="input-group">
                                        <label class="input-group-text">Product Group</label>
                                        <asp:DropDownList runat="server" ID="ddlProductGroup" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <div class="col-12 col-sm-12 col-lg-4">
                                    <div class="input-group">
                                        <label class="input-group-text">Method</label>
                                        <asp:DropDownList runat="server" ID="ddlMethod" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Cost" Text="Cost"></asp:ListItem>
                                            <asp:ListItem Value="Square Metre" Text="Square Metre"></asp:ListItem>
                                            <asp:ListItem Value="Linear Metre" Text="Linear Metre"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-12 col-sm-12 col-lg-8">
                                    <div class="input-group">
                                        <label class="input-group-text">Price Group</label>
                                        <asp:DropDownList runat="server" ID="ddlPriceGroup" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-12 col-sm-12 col-lg-4">
                                    <div class="input-group">
                                        <label class="input-group-text">Discount</label>
                                        <asp:TextBox runat="server" ID="txtDiscount" TextMode="Number" CssClass="form-control" placeholder="Discount ...." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card-footer d-flex justify-content-between">
                        <asp:Button runat="server" ID="btnSort" CssClass="btn btn-primary" Text="Sort" OnClick="btnSort_Click" />
                    </div>
                </div>
            </div>
        </section>

        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-12">
                                    <div class="grid-container table-responsive">
                                        <asp:GridView ID="gvList" runat="server" AutoGenerateColumns="true" CssClass="table table-bordered table-hover mb-0"></asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>