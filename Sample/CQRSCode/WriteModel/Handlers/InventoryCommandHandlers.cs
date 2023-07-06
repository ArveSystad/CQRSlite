using System.Threading;
using System.Threading.Tasks;
using CQRSCode.WriteModel.Commands;
using CQRSCode.WriteModel.Domain;
using CQRSlite.Commands;
using CQRSlite.Domain;

namespace CQRSCode.WriteModel.Handlers
{
    public class InventoryCommandHandlers : ICommandHandler<CreateInventoryItem>,
        ICancellableCommandHandler<DeactivateInventoryItem>,
        ICancellableCommandHandler<RemoveItemsFromInventory>,
        ICancellableCommandHandler<CheckInItemsToInventory>,
        ICancellableCommandHandler<RenameInventoryItem>
    {
        private readonly ISession _session;

        public InventoryCommandHandlers(ISession session)
        {
            _session = session;
        }

        public async Task Handle(CreateInventoryItem command)
        {
            var item = new InventoryItem(command.Id, command.Name);
            await _session.Add(item);
            await _session.Commit();
        }

        public async Task Handle(DeactivateInventoryItem command, CancellationToken token)
        {
            var item = await _session.Get<InventoryItem>(command.Id, command.ExpectedVersion, token);
            item.Deactivate();
            await _session.Commit(token);
        }

        public async Task Handle(RemoveItemsFromInventory command, CancellationToken token)
        {
            var item = await _session.Get<InventoryItem>(command.Id, command.ExpectedVersion, token);
            item.Remove(command.Count);
            await _session.Commit(token);
        }

        public async Task Handle(CheckInItemsToInventory command, CancellationToken token)
        {
            var item = await _session.Get<InventoryItem>(command.Id, command.ExpectedVersion, token);
            item.CheckIn(command.Count);
            await _session.Commit(token);
        }

        public async Task Handle(RenameInventoryItem command, CancellationToken token)
        {
            var item = await _session.Get<InventoryItem>(command.Id, command.ExpectedVersion, token);
            item.ChangeName(command.NewName);
            await _session.Commit(token);
        }
    }
}
