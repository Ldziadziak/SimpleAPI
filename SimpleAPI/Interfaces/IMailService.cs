﻿namespace SimpleAPI.Interfaces;
public interface IMailService
{
  void Send(string subject, string message);
}