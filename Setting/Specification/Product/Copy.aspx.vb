Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Product_Copy
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/product", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindDesignType()
            BindBlindType(ddlDesignType.SelectedValue)
            BindTubeType()
            BindControlType()
            BindCompanyDetail()
        End If
    End Sub

    Protected Sub ddlDesignType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindBlindType(ddlDesignType.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlDesignType.SelectedValue = "" Then
                MessageError(True, "DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlBlindType.SelectedValue = "" Then
                MessageError(True, "BLIND TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlTubeType.SelectedValue = "" Then
                MessageError(True, "TUBE TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlControlType.SelectedValue = "" Then
                MessageError(True, "CONTROL TYPE IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim blindString As String = String.Empty
                If Not ddlBlindType.SelectedValue = "" Then blindString = "AND BlindId='" & ddlBlindType.SelectedValue & "'"
                Dim tubeString As String = String.Empty
                If Not ddlTubeType.SelectedValue = "" Then tubeString = "AND TubeType='" & ddlTubeType.SelectedValue & "'"
                Dim controlString As String = String.Empty
                If Not ddlControlType.SelectedValue = "" Then controlString = "AND ControlType='" & ddlControlType.SelectedValue & "'"

                Dim thisString As String = String.Format("SELECT DISTINCT * FROM Products WHERE DesignId={0} {1} {2} {3} ORDER BY Id ASC", ddlDesignType.SelectedValue, blindString, tubeString, controlString)
                Dim thisData As DataTable = settingClass.GetDataTable(thisString)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/specification/product", False)
    End Sub

    Protected Sub BindDesignType()
        ddlDesignType.Items.Clear()
        Try
            ddlDesignType.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM Designs ORDER BY Name ASC")
            ddlDesignType.DataTextField = "Name"
            ddlDesignType.DataValueField = "Id"
            ddlDesignType.DataBind()

            If ddlDesignType.Items.Count > 1 Then
                ddlDesignType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindBlindType(designId As String)
        ddlBlindType.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(designId) Then
                ddlBlindType.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM Blinds WHERE DesignId='" & designId & "' ORDER BY Name ASC")
                ddlBlindType.DataTextField = "Name"
                ddlBlindType.DataValueField = "Id"
                ddlBlindType.DataBind()

                If ddlBlindType.Items.Count > 1 Then
                    ddlBlindType.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindTubeType()
        ddlTubeType.Items.Clear()
        Try
            ddlTubeType.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM ProductTubes ORDER BY Name ASC")
            ddlTubeType.DataTextField = "Name"
            ddlTubeType.DataValueField = "Id"
            ddlTubeType.DataBind()

            If ddlTubeType.Items.Count > 1 Then
                ddlTubeType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindControlType()
        ddlControlType.Items.Clear()
        Try
            ddlControlType.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM ProductControls ORDER BY Name ASC")
            ddlControlType.DataTextField = "Name"
            ddlControlType.DataValueField = "Id"
            ddlControlType.DataBind()

            If ddlControlType.Items.Count > 1 Then
                ddlControlType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompanyDetail()
        ddlCompanyDetail.Items.Clear()
        Try
            ddlCompanyDetail.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM CompanyDetails ORDER BY Name ASC")
            ddlCompanyDetail.DataTextField = "Name"
            ddlCompanyDetail.DataValueField = "Id"
            ddlCompanyDetail.DataBind()

            If ddlCompanyDetail.Items.Count > 1 Then
                ddlCompanyDetail.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
