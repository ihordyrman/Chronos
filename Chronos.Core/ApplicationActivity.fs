namespace Chronos.CoreF

open System

type ApplicationActivity =
    { ProcessName: string
      ApplicationName: string
      Title: string
      Start: DateTime
      End: DateTime option }
