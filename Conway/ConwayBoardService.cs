public interface IConwayBoardService {
    Dictionary<Guid, ConwayBoard> GetBoards();
    ConwayBoard GetBoard(Guid id);
    ConwayBoard CreateBoard(ConwayBoard b);

    List<List<bool>> GetFutureState(Guid id, int steps);
    List<List<bool>> GetFinalState(Guid id, int maxAttempts);
}

public class ConwayBoardService: IConwayBoardService {
    private readonly ConwaysGame _game = new();
    
    public Dictionary<Guid, ConwayBoard> GetBoards() {
        return _game.games;
    }

    public ConwayBoard CreateBoard(ConwayBoard b) {
        var boardId = _game.InitializeBoard(BoardSerializer.FromSerializableFormat(b.Board));
        return GetBoard(boardId);
    }

    public ConwayBoard GetBoard(Guid id) {
        return _game.GetBoard(id);
    }

    public List<List<bool>> GetFutureState(Guid id, int steps = 1) {
        var board = _game.GetBoard(id);
        return ConwaysGameMove.GetFutureState(board, steps);
    }

    public List<List<bool>> GetFinalState(Guid id, int maxAttempts = 100) {
        var board = _game.GetBoard(id);
        return ConwaysGameMove.GetFinalState(board, maxAttempts);
    }

}