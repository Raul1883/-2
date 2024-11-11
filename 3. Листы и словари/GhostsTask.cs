using System;
using System.Text;

namespace hashes;


//В этой практике необходимо сломать hash
public class GhostsTask :
    IFactory<Document>, IFactory<Vector>, IFactory<Segment>, IFactory<Cat>, IFactory<Robot>,
    IMagic
{
    // создаем объекты, которые будем ломать
    byte[] documentArray = { 1, 2, 3, 4 };
    Vector vector = new(0, 0);
    Segment segment = new(new Vector(1, 1), new Vector(2, 2));
    Cat cat = new("БАРСИК", "Кот", new DateTime(2011, 7, 21));
    Robot robot = new("idi");


    // создаем документ
    public Document Create()
    {
        return new Document("dock", Encoding.Unicode, documentArray);
    }


    // данный метод "ломает" hash
    public void DoMagic()
    {
        // т.к. hash считается через X, Y, меняем X,Y через метод Add
        vector.Add(new Vector(1, 1));

        // т.к. hash считается через Start, End, а эти поля - векторы,
        // можем "сломать" аналогично предыдущему пункту
        segment.Start.Add(vector);

        // т.к. hash считается через Name, а поле Name наследуется от класса Animal,
        // a Animal в свою очередь имеет публичное поле name можем сломать заменой
        // имени на любое другое
        cat.Rename("Мурка");

        // т.к. hash считается через BatteryCapacity, а BatteryCapacity публичное поле,
        // можем "сломать" просто поменяв это поле
        // декремент используется, чтобы менять hash каждый запуск тестов
        Robot.BatteryCapacity -= 1;

        // меняем состояния массива байт, чтобы "сломать" hash
        documentArray[0] = documentArray[1];
    }


    // далее идут интерфейсы, просто возвращающие значения
    Vector IFactory<Vector>.Create()
    {
        return vector;
    }

    Segment IFactory<Segment>.Create()
    {
        return segment;
    }

    Cat IFactory<Cat>.Create()
    {
        return cat;
    }

    Robot IFactory<Robot>.Create()
    {
        return robot;
    }
}