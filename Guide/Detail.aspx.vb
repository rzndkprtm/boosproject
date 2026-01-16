Imports System.Data

Partial Class Guide_Detail
    Inherits Page

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/guide", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("boos")) Then
            Response.Redirect("~/guide", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("boos").ToString()

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub BindData(guideId As String)
        Try
            Dim thisData As DataSet = settingClass.GetListData("SELECT * FROM Tutorials WHERE Id='" & guideId & "'")
            If thisData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/guide", False)
                Exit Sub
            End If

            hTitle.InnerText = thisData.Tables(0).Rows(0).Item("Title").ToString()
            spanDescription.InnerHtml = thisData.Tables(0).Rows(0).Item("Description").ToString()
            If String.IsNullOrEmpty(thisData.Tables(0).Rows(0).Item("Description").ToString()) Then
                spanDescription.InnerHtml = "<br /><br /><br /><br />"
            End If

            Dim videoName As String = thisData.Tables(0).Rows(0).Item("Video").ToString()
            If Not String.IsNullOrEmpty(videoName) Then
                frmVideo.Attributes("src") = videoName
            End If

            Dim pdfName As String = thisData.Tables(0).Rows(0).Item("File").ToString()
            If Not String.IsNullOrEmpty(pdfName) Then
                Dim linkFile As String = String.Format("~/Assets/tutorial/{0}", thisData.Tables(0).Rows(0).Item("File").ToString())
                frmPdf.Attributes("src") = ResolveUrl("~/Handler/PDF.ashx?document=" & linkFile)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
