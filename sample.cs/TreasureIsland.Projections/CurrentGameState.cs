using System;
using System.Collections.Generic;
using Glow.NotificationsCore;
using Glow.Sample.TreasureIsland.Domain;

namespace Glow.Sample;

public record CurrentGameState(Guid GameId, Dictionary<Guid, Player> Players, Game Game) : IClientNotification;