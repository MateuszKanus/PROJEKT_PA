#include <windows.h>
#include <string>   
#include <fstream>  
#include <vector>   
#include <sstream>
using namespace std;

//pojemniki na dane
vector<string> QCONTENT;
vector<string> ANSWER_1, ANSWER_2, ANSWER_3, ANSWER_4;
vector<string> CORRECT;
int POINTS_COUNT; //zmienna przechowujaca liczbe punktow
int ACTUAL_TASKS; //zmienna przechowujaca aktualny numer pytania
int ALL_TASKS;    //liczba pytan


//Definicje przyciskow do obslugi zdarzen
#define BUTTON_1 1
#define BUTTON_2 2
#define BUTTON_3 3
#define BUTTON_4 4
#define START_BUTTON 5

//Ustawienie wymiarow okna
#define main_Window_Width 800
#define main_Window_Height 600

//Instancja glowna programu
HINSTANCE hInstance;

//Uchwyty przyciskow
HWND H_BUTTON_1;
HWND H_BUTTON_2;
HWND H_BUTTON_3;
HWND H_BUTTON_4;
HWND H_START_BUTTON;

//Uchwyt tresci pytania
HWND H_LABEL_CONTENT;

//Label calkowita liczba pytan
HWND H_QUEST_COUNT;

//Label "Pytanie nr: "
HWND H_QUEST_LABEL;

//Konwersja int na string
template <class T>
string TO_STRING(T obj)
{
	stringstream SS;
	SS << obj;
	return SS.str();
}

enum STATUS {END_OF_QUEST};	//Typ wyliczeniowy

//Funkcja aktualizujaca numer pytania
void ACTUALIZE_QLABEL(void)
{
	string pytania;
	pytania = pytania + TO_STRING(ACTUAL_TASKS + 1) + "/" + TO_STRING(ALL_TASKS);
	SetWindowText(H_QUEST_COUNT, pytania.c_str());
}

//Funkcja ustawia tekst pytan
void SET_TEXT(int nr)
{
	//przy wyswietlaniu nowego pytania o aktualizujemy label z jego numerem
	ACTUALIZE_QLABEL();

	SetWindowText(H_LABEL_CONTENT, QCONTENT[nr].c_str());
	SetWindowText(H_BUTTON_1, ANSWER_1[nr].c_str());
	SetWindowText(H_BUTTON_2, ANSWER_2[nr].c_str());
	SetWindowText(H_BUTTON_3, ANSWER_3[nr].c_str());
	SetWindowText(H_BUTTON_4, ANSWER_4[nr].c_str());
}

//Funkcja sprawdza poprawnosc pytania, jezeli podana odpowiedz jest zla z odpowiedzia odczytana z pliku
bool CHECK_ANSWER(string SIGN)
{
	if (SIGN == CORRECT[ACTUAL_TASKS]) return 1;
	else return 0;
}

//Koniec gry
void END_GAME(int STATUS)
{
	string END;  
	END = END + "Wynik: " + TO_STRING(POINTS_COUNT) + "/" + TO_STRING(ALL_TASKS) + " pkt.";
	MessageBox(NULL, END.c_str(), "PA_PROJEKT", MB_ICONINFORMATION);
	PostQuitMessage(0); 
}

//Funkcja wywolana po kliknieciu przycisku z danym dla niego parametrem odpowiedzi
void MY_ANSWER(string SIGN)
{
	if (CHECK_ANSWER(SIGN))
	{
		POINTS_COUNT++;
	}

	ACTUAL_TASKS++;

	if ((ACTUAL_TASKS) >= ALL_TASKS)
	{
		END_GAME(END_OF_QUEST);
	}
	else SET_TEXT(ACTUAL_TASKS);
}

LRESULT CALLBACK wnd_proc(HWND window, UINT message, WPARAM wp, LPARAM lp)
{
	switch (message)
	{
	case WM_CLOSE:
		DestroyWindow(window);  //usuwanie obiektu window
		break;


	case WM_COMMAND:
		switch (wp)
		{
				//Reakcja na klikniecie przycisku START
				case START_BUTTON:

				//znisz przycisk start
				DestroyWindow(H_START_BUTTON);

				//Pokaz label tresci pytania oraz przyciskow
				H_LABEL_CONTENT = CreateWindowEx(0, "STATIC", NULL, WS_CHILD | WS_VISIBLE |
					SS_LEFT, 20, 13, 410, 20, window, NULL, hInstance, NULL);
				SetWindowText(H_LABEL_CONTENT, QCONTENT[ACTUAL_TASKS].c_str());
				H_BUTTON_1 = CreateWindowEx(WS_EX_CLIENTEDGE, "BUTTON", ANSWER_1[ACTUAL_TASKS].c_str(), WS_CHILD | WS_VISIBLE |
					WS_BORDER, 50, 80, 280, 40, window, (HMENU)BUTTON_1, hInstance, NULL);
				H_BUTTON_2 = CreateWindowEx(WS_EX_CLIENTEDGE, "BUTTON", ANSWER_2[ACTUAL_TASKS].c_str(), WS_CHILD | WS_VISIBLE |
					WS_BORDER, 50, 130, 280, 40, window, (HMENU)BUTTON_2, hInstance, NULL);
				H_BUTTON_3 = CreateWindowEx(WS_EX_CLIENTEDGE, "BUTTON", ANSWER_3[ACTUAL_TASKS].c_str(), WS_CHILD | WS_VISIBLE |
					WS_BORDER, 50, 180, 280, 40, window, (HMENU)BUTTON_3, hInstance, NULL);
				H_BUTTON_4 = CreateWindowEx(WS_EX_CLIENTEDGE, "BUTTON", ANSWER_4[ACTUAL_TASKS].c_str(), WS_CHILD | WS_VISIBLE |
					WS_BORDER, 50, 230, 280, 40, window, (HMENU)BUTTON_4, hInstance, NULL);

				//utworz label punktow - tekst
				H_QUEST_LABEL = CreateWindowEx(0, "STATIC", NULL, WS_CHILD | WS_VISIBLE |
					SS_LEFT, 500, 70, 120, 20, window, NULL, hInstance, NULL);
				SetWindowText(H_QUEST_LABEL, "Numer pytania: ");

				//label punktow - liczba
				H_QUEST_COUNT = CreateWindowEx(0, "STATIC", NULL, WS_CHILD | WS_VISIBLE |
					SS_LEFT, 500, 110, 120, 20, window, NULL, hInstance, NULL);
				ACTUALIZE_QLABEL();

				break;

			//Odpowiedz a
			case BUTTON_1:
			MY_ANSWER("a");
			break;

			//Odpowiedz b
			case BUTTON_2:
			MY_ANSWER("b");
			break;

			//Odpowiedz c
			case BUTTON_3:
			MY_ANSWER("c");
			break;

			//Odpowiedz d
			case BUTTON_4:
			MY_ANSWER("d");
			break;
		}
		break;

	case WM_DESTROY:
		PostQuitMessage(0);
		break;

	default:
		return DefWindowProc(window, message, wp, lp);
	}
	return 0;
}

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE, LPSTR, int)
{
	int LINE_NO = 1;     //zmienna nr lini do odczytu pliku
	string LINE;       //lancuch - bufor pojedynczej linii pliku
	int QUESTION_NO = 0;   //zmienna nr pytania do odczytu pliku

	fstream FILE;       //utworzenie obiektu FILE typu fstream
	FILE.open("quiz.txt", ios::in);  //otwarcie pliku, ios::in

	//Petla while odczytuje z pliku do momentu napotkania EOF
	while (getline(FILE, LINE))
	{
		switch (LINE_NO)  
		{
		case 1: QCONTENT.push_back(LINE);      break;    
		case 2: ANSWER_1.push_back(LINE);      break;    
		case 3: ANSWER_2.push_back(LINE);      break;    
		case 4: ANSWER_3.push_back(LINE);      break;
		case 5: ANSWER_4.push_back(LINE);      break;
		case 6: CORRECT.push_back(LINE);	   break;
		}

		//jezeli numer lini = 6 czyli ostatni dla danego pytania po wyzeruj nr linii i zwieksz nr pytania
		if (LINE_NO == 6) { LINE_NO = 0; QUESTION_NO++; }    //poczotkowo numer pytania = 0, a jak LINE = 6 to numer pytania++ czyli 1
		LINE_NO++; //zwiekszenie numeru linii, zawsze zacznie sie od 1
	}

	FILE.close();

	ALL_TASKS = QCONTENT.size();

	//rejestracja klasy okienkowej
	WNDCLASSEX wc;
	wc.cbSize = sizeof(WNDCLASSEX);
	wc.cbClsExtra = 0;
	wc.style = 0;
	wc.lpfnWndProc = wnd_proc;
	wc.cbWndExtra = 0;
	wc.hInstance = hInstance;
	wc.hIcon = LoadIcon(NULL, IDI_APPLICATION);
	wc.hIconSm = LoadIcon(NULL, IDI_APPLICATION);
	wc.hCursor = LoadCursor(NULL, IDC_ARROW);
	wc.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
	wc.lpszMenuName = NULL;
	wc.lpszClassName = "NAZWA_KLASY";
	RegisterClassEx(&wc);

	//tworzenie okna...
	HWND window = CreateWindowEx(0, "NAZWA_KLASY", "PA_PROJEKT",
		WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_VISIBLE,
		CW_USEDEFAULT, CW_USEDEFAULT, main_Window_Width, main_Window_Height,
		NULL, NULL, hInstance, NULL);
	if (NULL == window) return -1; 


								   //Pierwszy zostanie utworzony przycisk START
	H_START_BUTTON = CreateWindowEx(WS_EX_CLIENTEDGE, "BUTTON", "START", WS_CHILD | WS_VISIBLE |
		WS_BORDER, main_Window_Width / 2 - 250 / 2, main_Window_Height / 2 - 80 / 2, 250, 80, window, (HMENU)START_BUTTON, hInstance, NULL);

MSG msg;
while (GetMessage(&msg, 0, 0, 0))
{
	TranslateMessage(&msg);
	DispatchMessage(&msg);
}

	return 1;
}




