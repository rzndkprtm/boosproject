<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_General_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Master General" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .card-clickable {
            cursor: pointer;
            transition: transform 0.15s ease, box-shadow 0.15s ease;
            }
        
        .card-clickable:hover {
            transform: translateY(-3px);
            box-shadow: 0 4px 12px rgba(0,0,0,.15);
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
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divCompany">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Company</h6>
                                <h6 class="font-extrabold mb-0" runat="server"><%= GetSumData("Companys") %></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divMailing">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Mailing</h6>
                                <h6 class="font-extrabold mb-0"><%= GetSumData("Mailings") %></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divRoleAccess">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Role Access</h6>
                                <h6 class="font-extrabold mb-0"><%= GetSumData("CustomerLoginRoles") %></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divLevelAccess">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Level Access</h6>
                                <h6 class="font-extrabold mb-0"><%= GetSumData("CustomerLoginLevels") %></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divNewsletter">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Newsletter</h6>
                                <h6 class="font-extrabold mb-0"><%= GetSumData("Newsletters") %></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divTutorial">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Tutorial</h6>
                                <h6 class="font-extrabold mb-0"><%= GetSumData("Tutorials") %></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card card-clickable" runat="server" id="divActionAccess">
                    <div class="card-body px-3 py-4-5">
                        <div class="row">
                            <div class="col-4">
                                <div class="stats-icon purple">
                                    <i class="iconly-boldShow"></i>
                                </div>
                            </div>
                            <div class="col-8">
                                <h6 class="text-muted font-semibold">Action Access</h6>
                                <h6 class="font-extrabold mb-0"><%= GetSumData("Actions") %></h6>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>