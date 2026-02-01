Imports System.Data
Imports System.Data.SqlClient

Public Partial Class SiteMaster
    Inherits MasterPage

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing

    Dim settingClass As New SettingClass
    Dim mailingClass As New MailingClass

    Protected Sub Page_Init(sender As Object, e As EventArgs)
        AddHandler Page.PreLoad, AddressOf master_Page_PreLoad
    End Sub

    Protected Sub master_Page_PreLoad(sender As Object, e As EventArgs)
        CheckSessions()
        MyLoad()
        BindListNavigation()
    End Sub

    Protected Sub linkLogout_Click(sender As Object, e As EventArgs)
        Dim sessionId As String = String.Empty

        If Request.Cookies("deviceId") IsNot Nothing Then
            sessionId = Request.Cookies("deviceId").Value
            settingClass.DeleteSession(sessionId)
        End If
        Session.Clear()
        Response.Redirect("~/account/login", True)
    End Sub

    Private Sub MyLoad()
        Try
            If Session("IsLoggedIn") = True Then
                Dim loginId As String = Session("LoginId")

                Dim params As New List(Of SqlParameter) From {
                    New SqlParameter("@LoginId", loginId)
                }
                Dim myData As DataRow = settingClass.GetDataRowSP("sp_LoginProfile", params)
                If myData Is Nothing Then
                    Session.Clear()
                    Response.Redirect("~/account/login", False)
                    Exit Sub
                End If

                Session("CustomerId") = myData("CustomerId").ToString()
                Session("CustomerLevel") = myData("CustomerLevel").ToString()

                Session("RoleId") = myData("RoleId").ToString()
                Session("LevelId") = myData("LevelId").ToString()
                Session("FullName") = myData("FullName").ToString()
                Session("PersonalEmail") = myData("Email").ToString()
                Session("ResetLogin") = myData("ResetLogin")
                Session("PriceAccess") = myData("PriceAccess").ToString()

                Session("CompanyId") = myData("CompanyId").ToString()
                Session("CompanyDetailId") = myData("CompanyDetailId").ToString()

                Session("RoleName") = myData("RoleName").ToString()
                Session("LevelName") = myData("LevelName").ToString()
                Session("CompanyName") = myData("CompanyName").ToString()

                Dim companyId As String = myData("CompanyId").ToString()
                Dim companyActive As Boolean = myData("CompanyActive")
                Dim customerActive As Boolean = myData("CustomerActive")
                Dim loginActive As Boolean = myData("Active")
                Dim roleActive As Boolean = myData("RoleActive")
                Dim levelActive As Boolean = myData("LevelActive")
                Dim resetLogin As Boolean = myData("ResetLogin")
                Dim personalEmail As String = myData("Email").ToString()

                If customerActive = False Then
                    Response.Redirect("~/error", False)
                    Exit Sub
                End If

                If companyActive = False Then
                    Response.Redirect("~/error", False)
                    Exit Sub
                End If

                If roleActive = False Then
                    Response.Redirect("~/error", False)
                    Exit Sub
                End If

                If resetLogin = True AndAlso Not Request.Url.AbsolutePath.ToLower().EndsWith("/account/password") Then
                    Response.Redirect("~/account/password", False)
                    Exit Sub
                End If

                If companyId = "1" Then
                    imgLogo.ImageUrl = "~/assets/images/logo/general.jpg"
                End If
                If companyId = "2" Then
                    imgLogo.ImageUrl = "~/assets/images/logo/jpmdirect.jpg"
                End If
                If companyId = "3" Then
                    imgLogo.ImageUrl = "~/assets/images/logo/accent.png"
                End If
                If companyId = "4" Then
                    imgLogo.ImageUrl = "~/assets/images/logo/general.jpg"
                End If
                If companyId = "5" Then
                    imgLogo.ImageUrl = "~/assets/images/logo/general.jpg"
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("sp_UpdateCustomerLastLogin", thisConn)
                        myCmd.CommandType = CommandType.StoredProcedure
                        myCmd.Parameters.Add("@Id", SqlDbType.Int).Value = loginId

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using
            End If
        Catch ex As Exception
            Session.Clear()
            Response.Redirect("~/account/login", False)
        End Try
    End Sub

    Private Sub BindListNavigation()
        Try
            liOldOrder.Visible = False
            liGuide.Visible = False
            liTicket.Visible = False
            liExport.Visible = False
            liReport.Visible = False
            liSales.Visible = False

            liSettingQuote.Visible = False

            liSetting.Visible = False

            liGeneral.Visible = False
            liGeneralCompany.Visible = False
            liGeneralMailing.Visible = False
            liGeneralRole.Visible = False
            liGeneralLevel.Visible = False
            liGeneralNewsletter.Visible = False
            liGeneralTutorial.Visible = False
            liGeneralAccess.Visible = False

            liCustomerDev.Visible = False
            liCustomer.Visible = False

            liSpecification.Visible = False
            liSpecificationDesign.Visible = False
            liSpecificationBlind.Visible = False
            liSpecificationProduct.Visible = False
            liSpecificationTube.Visible = False
            liSpecificationControl.Visible = False
            liSpecificationColour.Visible = False
            liSpecificationFabric.Visible = False
            liSpecificationChain.Visible = False
            liSpecificationBottom.Visible = False
            liSpecificationMounting.Visible = False

            liPrice.Visible = False
            liPriceGroup.Visible = False
            liPriceProductGroup.Visible = False
            liPriceBase.Visible = False
            liPriceSurcharge.Visible = False
            liPricePromo.Visible = False

            liAdditional.Visible = False

            If Session("RoleName") = "Developer" Then
                liOldOrder.Visible = True
                liGuide.Visible = True
                liTicket.Visible = True
                liExport.Visible = True
                liReport.Visible = True
                liSales.Visible = True

                liSetting.Visible = True

                liGeneral.Visible = True
                liGeneralCompany.Visible = True
                liGeneralMailing.Visible = True
                liGeneralRole.Visible = True
                liGeneralLevel.Visible = True
                liGeneralNewsletter.Visible = True
                liGeneralTutorial.Visible = True
                liGeneralAccess.Visible = True

                liCustomerDev.Visible = True

                liSpecification.Visible = True
                liSpecificationDesign.Visible = True
                liSpecificationBlind.Visible = True
                liSpecificationProduct.Visible = True
                liSpecificationTube.Visible = True
                liSpecificationControl.Visible = True
                liSpecificationColour.Visible = True
                liSpecificationFabric.Visible = True
                liSpecificationChain.Visible = True
                liSpecificationBottom.Visible = True
                liSpecificationMounting.Visible = True

                liPrice.Visible = True
                liPriceGroup.Visible = True
                liPriceProductGroup.Visible = True
                liPriceBase.Visible = True
                liPriceSurcharge.Visible = True
                liPricePromo.Visible = True

                liAdditional.Visible = True
            End If

            If Session("RoleName") = "IT" Then
                liOldOrder.Visible = True
                liExport.Visible = True
                liReport.Visible = True
                liSales.Visible = True
                liTicket.Visible = True
                liGuide.Visible = True

                liSetting.Visible = True

                liGeneral.Visible = True
                liGeneralCompany.Visible = True
                liGeneralMailing.Visible = True
                liGeneralRole.Visible = True
                liGeneralLevel.Visible = True
                liGeneralNewsletter.Visible = True
                liGeneralTutorial.Visible = True

                liCustomer.Visible = True

                liSpecification.Visible = True
                liSpecificationDesign.Visible = True
                liSpecificationBlind.Visible = True
                liSpecificationProduct.Visible = True
                liSpecificationTube.Visible = True
                liSpecificationControl.Visible = True
                liSpecificationColour.Visible = True
                liSpecificationFabric.Visible = True
                liSpecificationChain.Visible = True
                liSpecificationBottom.Visible = True
                liSpecificationMounting.Visible = True

                liPrice.Visible = True
                liPriceGroup.Visible = True
                liPriceProductGroup.Visible = True
                liPriceBase.Visible = True
                liPriceSurcharge.Visible = True
                liPricePromo.Visible = True
            End If

            If Session("RoleName") = "Factory Office" Then
                liOldOrder.Visible = True
                liReport.Visible = True
                liSales.Visible = True
                liTicket.Visible = True
                liGuide.Visible = True

                liSetting.Visible = True

                liGeneral.Visible = True
                liGeneralCompany.Visible = True
                liGeneralMailing.Visible = True
                liGeneralNewsletter.Visible = True
                liGeneralTutorial.Visible = True

                liCustomer.Visible = True

                liSpecification.Visible = True
                liSpecificationDesign.Visible = True
                liSpecificationBlind.Visible = True
                liSpecificationProduct.Visible = True
                liSpecificationFabric.Visible = True
                liSpecificationChain.Visible = True
                liSpecificationBottom.Visible = True
                liSpecificationMounting.Visible = True

                liPrice.Visible = True
                liPriceGroup.Visible = True
                liPriceProductGroup.Visible = True
                liPriceBase.Visible = True
                liPriceSurcharge.Visible = True
                liPricePromo.Visible = True
            End If

            If Session("RoleName") = "Production" Then
                liGuide.Visible = True
                liTicket.Visible = True
            End If

            If Session("RoleName") = "Sales" Then
                liOldOrder.Visible = True
                liTicket.Visible = True
                liGuide.Visible = True

                If Session("LevelName") = "Leader" Then
                    liReport.Visible = True
                    liSales.Visible = True
                End If

                liSetting.Visible = True

                liCustomer.Visible = True
            End If

            If Session("RoleName") = "Account" Then
                liOldOrder.Visible = True
                liTicket.Visible = True
                liGuide.Visible = True

                liSetting.Visible = True

                liCustomer.Visible = True

                liSpecification.Visible = True
                liSpecificationFabric.Visible = True

                liPrice.Visible = True
                liPriceGroup.Visible = True
                liPriceProductGroup.Visible = True
                liPriceBase.Visible = True
                liPricePromo.Visible = True
            End If

            If Session("RoleName") = "Customer Service" Then
                liOldOrder.Visible = True
                liReport.Visible = True
                liTicket.Visible = True
                liGuide.Visible = True

                liSetting.Visible = True

                liCustomer.Visible = True

                liSpecification.Visible = True
                liSpecificationProduct.Visible = True
                liSpecificationFabric.Visible = True
                liSpecificationChain.Visible = True
                liSpecificationBottom.Visible = True
                liSpecificationMounting.Visible = True
            End If

            If Session("RoleName") = "Customer" Then
                liOldOrder.Visible = True
                liGuide.Visible = True
                liTicket.Visible = True

                liSettingQuote.Visible = True
            End If

            If Session("RoleName") = "Data Entry" Then
                liGuide.Visible = True
                liSetting.Visible = True
                liCustomer.Visible = True
            End If

            If Session("RoleName") = "Installer" Then

            End If
        Catch ex As Exception
            Session.Clear()
            Response.Redirect("~/account/login", False)
        End Try
    End Sub

    Private Sub CheckSessions()
        Try
            Dim sessionId As String = String.Empty
            If Session("IsLoggedIn") = True Then
                If Request.Cookies("deviceId") IsNot Nothing Then
                    sessionId = Request.Cookies("deviceId").Value
                    Dim checkData As DataRow = settingClass.GetDataRow("SELECT * FROM Sessions WHERE Id='" & UCase(sessionId) & "' AND LoginId='" & Session("LoginId") & "'")

                    If checkData Is Nothing Then
                        Response.Redirect("~/account/login", True)
                        Exit Sub
                    End If
                Else
                    Response.Redirect("~/account/login", True)
                    Exit Sub
                End If
            Else
                If Request.Cookies("deviceId") IsNot Nothing Then
                    sessionId = Request.Cookies("deviceId").Value
                    Dim loginId As String = settingClass.GetItemData("SELECT LoginId FROM Sessions WHERE Id='" & UCase(sessionId).ToString() & "'")
                    If Not loginId = "" Then
                        Dim userName As String = settingClass.GetItemData("SELECT UserName FROM CustomerLogins WHERE Id='" & loginId & "'")

                        Session.Add("IsLoggedIn", True)
                        Session.Add("LoginId", loginId)
                        Session.Add("UserName", userName)

                        Response.Redirect("~/", True)
                        Exit Sub
                    Else
                        Response.Redirect("~/account/login", True)
                        Exit Sub
                    End If
                Else
                    Response.Redirect("~/account/login", True)
                    Exit Sub
                End If
            End If
        Catch ex As Exception
            Response.Redirect("~/account/login", True)
            Exit Sub
        End Try
    End Sub
End Class
