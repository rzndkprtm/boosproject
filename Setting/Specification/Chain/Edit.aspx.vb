Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Chain_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/chain", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("chainid")) Then
            Response.Redirect("~/setting/specification/chain", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("chainid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If Not String.IsNullOrEmpty(txtBoeId.Text) Then
                If Not IsNumeric(txtBoeId.Text) Then
                    MessageError(True, "BOE ID SHOULD BE NUMERIC !")
                    Exit Sub
                End If
            End If
            If txtName.Text = "" Then
                MessageError(True, "CHAIN / REMOTE NAME IS REQUIRED !")
                Exit Sub
            End If
            If lbDesign.SelectedValue = "" Then
                MessageError(True, "DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If
            If lbCompanyDetail.SelectedValue = "" Then
                MessageError(True, "SUB COMPANY IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Dim selectedDesign As String = String.Empty
                For Each item As ListItem In lbDesign.Items
                    If item.Selected Then
                        selectedDesign += item.Value & ","
                    End If
                Next

                Dim selectedCompany As String = String.Empty
                For Each item As ListItem In lbCompanyDetail.Items
                    If item.Selected Then
                        selectedCompany += item.Value & ","
                    End If
                Next

                Dim designType As String = selectedDesign.Remove(selectedDesign.Length - 1).ToString()
                Dim companyDetail As String = selectedCompany.Remove(selectedCompany.Length - 1).ToString()
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE Chains SET BoeId=@BoeId, Name=@Name, DesignId=@DesignId, ControlTypeId='1', CompanyDetailId=@CompanyDetailId, ChainType=@ChainType, ChainLength=@ChainLength, Description=@Description WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), CType(DBNull.Value, Object), txtBoeId.Text))
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@DesignId", designType)
                        thisCmd.Parameters.AddWithValue("@CompanyDetailId", companyDetail)
                        thisCmd.Parameters.AddWithValue("@ChainType", ddlChainType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@ChainLength", ddlChainLength.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Chains", lblId.Text, Session("LoginId").ToString(), "Chain Updated"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/specification/chain", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/specification/chain", False)
    End Sub

    Protected Sub BindData(chainId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Chains WHERE Id='" & chainId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/specification/chain", False)
                Exit Sub
            End If

            BindDesign()
            BindCompanyDetail()

            txtBoeId.Text = thisData("BoeId").ToString()
            txtName.Text = thisData("Name").ToString()
            ddlChainType.SelectedValue = thisData("ChainType").ToString()
            ddlChainLength.SelectedValue = thisData("ChainLength").ToString()
            txtDescription.Text = thisData("Description").ToString()

            If Not thisData("DesignId").ToString() = "" Then
                Dim thisArray() As String = thisData("DesignId").ToString().Split(",")
                For Each i In thisArray
                    If Not (i.Equals(String.Empty)) Then
                        lbDesign.Items.FindByValue(i).Selected = True
                    End If
                Next
            End If

            If Not thisData("CompanyDetailId").ToString() = "" Then
                Dim thisArray() As String = thisData("CompanyDetailId").ToString().Split(",")
                For Each i In thisArray
                    If Not (i.Equals(String.Empty)) Then
                        lbCompanyDetail.Items.FindByValue(i).Selected = True
                    End If
                Next
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindDesign()
        lbDesign.Items.Clear()
        Try
            lbDesign.DataSource = settingClass.GetDataTable("SELECT * FROM Designs CROSS APPLY STRING_SPLIT(AppliesTo, ',') applyArray WHERE applyArray.VALUE='Chains' ORDER BY Name ASC")
            lbDesign.DataTextField = "Name"
            lbDesign.DataValueField = "Id"
            lbDesign.DataBind()

            If lbDesign.Items.Count > 0 Then
                lbDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompanyDetail()
        lbCompanyDetail.Items.Clear()
        Try
            lbCompanyDetail.DataSource = settingClass.GetDataTable("SELECT * FROM CompanyDetails WHERE Status='Active' OR Status='Inactive' ORDER BY Name ASC")
            lbCompanyDetail.DataTextField = "Name"
            lbCompanyDetail.DataValueField = "Id"
            lbCompanyDetail.DataBind()

            If lbCompanyDetail.Items.Count > 0 Then
                lbCompanyDetail.Items.Insert(0, New ListItem("", ""))
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
