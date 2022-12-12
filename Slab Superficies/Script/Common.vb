﻿Imports System.Console
Imports System.ConsoleColor
Imports System.Diagnostics.Process
Imports System.IO
Imports System.IO.Directory
Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Threading.Thread
Imports System.Windows.Forms
Imports System.Windows.Forms.MessageBox
Imports System.Windows.Forms.MessageBoxButtons
Imports System.Windows.Forms.MessageBoxDefaultButton
Imports System.Windows.Forms.MessageBoxIcon

Friend Module Common
#Region "Helper"
    ''' <summary>
    ''' Check internet connection.
    ''' </summary>
    ''' <returns>Connection state.</returns>
    Private Function IsNetAvail()
        Dim objResp As WebResponse
        Try
            objResp = WebRequest.Create(New Uri(My.Resources.link_base)).GetResponse
            objResp.Close()
            objResp = Nothing
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Check update.
    ''' </summary>
    Private Sub ChkUpd()
        HdrSty("アップデートの確認...")
        If IsNetAvail() AndAlso Not (New WebClient).DownloadString(My.Resources.link_ver).Contains(My.Resources.app_ver) Then
            MsgBox($"「{My.Resources.app_true_name}」新しいバージョンが利用可能！", 262144, Title:="更新")
            Dim frmUpd = New FrmUpdate
            frmUpd.ShowDialog()
        End If
    End Sub

    ''' <summary>
    ''' Update valid license.
    ''' </summary>
    Friend Sub UpdVldLic()
        My.Settings.Chk_Key = True
        My.Settings.Save()
    End Sub

    ''' <summary>
    ''' Fade in form.
    ''' </summary>
    <Extension()>
    Friend Sub FIFrm(frm As Form)
        While frm.Opacity < 1
            frm.Opacity += 0.05
            frm.Update()
            Sleep(10)
        End While
    End Sub

    ''' <summary>
    ''' Fade out form.
    ''' </summary>
    <Extension()>
    Friend Sub FOFrm(frm As Form)
        While frm.Opacity > 0
            frm.Opacity -= 0.05
            frm.Update()
            Sleep(10)
        End While
    End Sub
#End Region

#Region "Master"
    ''' <summary>
    ''' End process.
    ''' </summary>
    ''' <param name="name">Process name.</param>
    Friend Sub KillPrcs(name As String)
        If GetProcessesByName(name).Count > 0 Then
            For Each item In GetProcessesByName(name)
                item.Kill()
            Next
        End If
    End Sub

    ''' <summary>
    ''' Run application.
    ''' </summary>
    Private Sub RunApp()
        Intro()
        Dim n = InpD(vbTab & "面積の数量: ")
        Dim area(n) As Area
        For i = 0 To n - 1
            area(i) = New Area
            ' Input ws
            Logger(area, i)
            area(i).W = InpG("w = ")
            If Not area(i).W > 0 Then
                If i > 0 Then
                    area(i - 1).H = 0
                    i -= 1
                Else
                    i -= 1
                    Continue For
                End If
            End If
            ' Input hs
            Logger(area, i)
            area(i).H = InpG(vbTab & "h = ")
            If Not area(i).H > 0 Then
                area(i).H = 0
                i -= 1
                Continue For
            End If
        Next
        Dim areaS(n) As Double
        Dim sigS = 0D
        For i = 0 To n - 1
            areaS(i) = area(i).SArea()
            sigS += areaS(i)
        Next
        ' Output
        Dim fmt = "{0," + sigS.ToString().Length.ToString() + ":####.#}"
        Intro()
        For i = 0 To n - 1
            HxSty(vbTab & $"Ｓ{i + 1} (m²)" & vbTab & vbTab & ": " + String.Format(fmt, areaS(i)) & vbCrLf)
        Next
        RsltSty(vbTab & "ΣＳ (m²)" & vbTab & vbTab & ": " + String.Format(fmt, sigS) & vbCrLf)
        Credit()
    End Sub

    ''' <summary>
    ''' First run.
    ''' </summary>
    Friend Sub FstRunApp()
        ChkUpd()
        RunApp()
    End Sub
#End Region

#Region "Main"
    ''' <summary>
    ''' Create directory advanced.
    ''' </summary>
    ''' <param name="path">Folder path.</param>
    Friend Sub CrtDirAdv(path As String)
        If Not Exists(path) Then
            CreateDirectory(path)
        End If
    End Sub

    ''' <summary>
    ''' Delete file advanced.
    ''' </summary>
    ''' <param name="path">File path.</param>
    Friend Sub DelFileAdv(path As String)
        If File.Exists(path) Then
            File.Delete(path)
        End If
    End Sub

    ''' <summary>
    ''' Convert to G.
    ''' </summary>
    ''' <param name="num">Number.</param>
    ''' <returns>Number converted.</returns>
    Private Function ConvertToG(num As Double)
        Return If(num < 30, num * 910, num)
    End Function

    ''' <summary>
    ''' Write log.
    ''' </summary>
    ''' <param name="length">Length of array.</param>
    Private Sub Logger(arrArea As Area(), length As Integer)
        Intro()
        Dim sHx = ""
        For j = 0 To length
            sHx += $"{j + 1}) "
            If arrArea(j).W > 0 Then
                sHx += $"w = {arrArea(j).W}"
            Else
                Exit For
            End If
            If arrArea(j).H > 0 Then
                sHx += vbTab & $"h = {arrArea(j).H}" & vbCrLf
            Else
                Exit For
            End If
        Next
        HxSty(sHx)
    End Sub
#End Region

#Region "Timer"
    ''' <summary>
    ''' Start timer advanced.
    ''' </summary>
    <Extension()>
    Friend Sub StrtAdv(tmr As Timer)
        If Not tmr.Enabled Then
            tmr.Start()
        End If
    End Sub

    ''' <summary>
    ''' Stop timer advanced.
    ''' </summary>
    <Extension()>
    Friend Sub StopAdv(tmr As Timer)
        If tmr.Enabled Then
            tmr.Stop()
        End If
    End Sub
#End Region

#Region "Actor"
    ''' <summary>
    ''' Header style.
    ''' </summary>
    ''' <param name="caption">Caption.</param>
    Private Sub HdrSty(caption As String)
        ForegroundColor = DarkYellow
        Write(caption)
    End Sub

    ''' <summary>
    ''' Intro style.
    ''' </summary>
    ''' <param name="caption">Caption.</param>
    Private Sub IntroSty(caption As String)
        ForegroundColor = Blue
        Write(caption)
    End Sub

    ''' <summary>
    ''' Credit style.
    ''' </summary>
    ''' <param name="caption">Caption.</param>
    Private Sub CreditSty(caption As String)
        ForegroundColor = DarkGray
        Write(caption)
    End Sub

    ''' <summary>
    ''' Title style.
    ''' </summary>
    ''' <param name="caption">Caption.</param>
    Private Sub TitSty(caption As String)
        ForegroundColor = Green
        Write(caption)
    End Sub

    ''' <summary>
    ''' Input style.
    ''' </summary>
    ''' <param name="caption">Caption.</param>
    Private Sub InpSty(caption As String)
        ForegroundColor = Cyan
        Write(caption)
    End Sub

    ''' <summary>
    ''' History style.
    ''' </summary>
    ''' <param name="caption">Caption.</param>
    Private Sub HxSty(caption As String)
        ForegroundColor = Gray
        Write(caption)
    End Sub

    ''' <summary>
    ''' Result style.
    ''' </summary>
    ''' <param name="caption">Caption.</param>
    Private Sub RsltSty(caption As String)
        ForegroundColor = DarkCyan
        Write(caption)
    End Sub

    ''' <summary>
    ''' Error style.
    ''' </summary>
    ''' <param name="caption">Caption.</param>
    Friend Sub ErrSty(caption As String)
        ForegroundColor = Red
        Write(caption)
    End Sub

    ''' <summary>
    ''' Prefix input.
    ''' </summary>
    ''' <param name="caption">Caption.</param>
    Private Sub PrefInp(caption As String)
        InpSty(caption)
        ForegroundColor = White
    End Sub

    ''' <summary>
    ''' Intro.
    ''' </summary>
    Private Sub Intro()
        Clear()
        IntroSty(My.Resources.gr_name & vbCrLf)
        IntroSty(My.Resources.cc_text & vbCrLf)
        TitSty(vbCrLf & My.Resources.app_true_name & vbCrLf & vbCrLf)
    End Sub

    ''' <summary>
    ''' Credit.
    ''' </summary>
    Private Sub Credit()
        CreditSty(vbCrLf & "続行するには、任意のキーを押してください...")
        ReadKey()
        If Show("続けたいですか？", "質問", YesNo, Question, Button2) = DialogResult.Yes Then
            RunApp()
        End If
    End Sub

    ''' <summary>
    ''' Input double.
    ''' </summary>
    ''' <param name="caption">Caption.</param>
    ''' <returns>Input value.</returns>
    Private Function InpD(caption As String)
        PrefInp(caption)
        Return Val(ReadLine)
    End Function

    ''' <summary>
    ''' Input G.
    ''' </summary>
    ''' <param name="caption">Caption.</param>
    ''' <returns>Input value.</returns>
    Private Function InpG(caption As String)
        PrefInp(caption)
        Return ConvertToG(Val(ReadLine))
    End Function
#End Region
End Module
