Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Validation_Detail_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/validation/detail", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("detailid")) Then
            Response.Redirect("~/setting/validation/detail", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("detailid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE ValidationDetails SET GroupNo=@GroupNo, FieldName=@FieldName, Operator=@Operator, CompareValue=@CompareValue, DataType=@DataType WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    thisCmd.Parameters.AddWithValue("@GroupNo", ddlGroupNo.SelectedValue)
                    thisCmd.Parameters.AddWithValue("@FieldName", ddlFieldName.SelectedValue)
                    thisCmd.Parameters.AddWithValue("@Operator", ddlOperator.SelectedValue)
                    thisCmd.Parameters.AddWithValue("@CompareValue", txtCompareValue.Text)
                    thisCmd.Parameters.AddWithValue("@DataType", ddlDataType.SelectedValue)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            'dataLog = {"ValidationDetails", lblId.Text, Session("LoginId").ToString(), "Validation Detail Updated"}
            'settingClass.Logs(dataLog)

            Dim url As String = String.Format("~/setting/validation/detail?validationid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Dim url As String = String.Format("~/setting/validation/detail?validationid={0}", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindData(detailId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM ValidationDetails WHERE Id='" & detailId & "'")
            If thisData Is Nothing Then Exit Sub

            BindFieldName()
            BindOperator()

            ddlGroupNo.SelectedValue = thisData("GroupNo").ToString()
            ddlFieldName.SelectedValue = thisData("FieldName").ToString()
            ddlOperator.SelectedValue = thisData("Operator").ToString()
            txtCompareValue.Text = thisData("CompareValue").ToString()
            ddlDataType.SelectedValue = thisData("DataType").ToString()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindFieldName()
        Try
            ddlFieldName.DataSource = SettingClass.ListValidationFieldName
            ddlFieldName.DataBind()
            ddlFieldName.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindOperator()
        Try
            ddlOperator.DataSource = SettingClass.ListValidationOperator
            ddlOperator.DataBind()
            ddlOperator.Items.Insert(0, New ListItem("", ""))
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